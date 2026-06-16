using FluentAssertions;
using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Application.Services;
using IntegrationPlatform.Contracts.Requests;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
using IntegrationPlatform.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
namespace IntegrationPlatform.UnitTests.Services;
public class OperationServiceTests
{
    private readonly Mock<IOperationRepository> _repositoryMock;
    private readonly Mock<IOperationHandler> _handlerMock;
    private readonly OperationService _sut;
    public OperationServiceTests()
    {
        _repositoryMock = new Mock<IOperationRepository>();
        _handlerMock = new Mock<IOperationHandler>();
        _handlerMock.Setup(h => h.OperationType).Returns(OperationType.DataSynchronization);
        _handlerMock.Setup(h => h.ExecuteAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>())).ReturnsAsync("success");
        _sut = new OperationService(_repositoryMock.Object, [_handlerMock.Object], NullLogger<OperationService>.Instance);
    }
    [Fact]
    public async Task ExecuteSyncAsync_CreatesOperationAndReturnsCompleted()
    {
        var request = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "test",
            MaxRetries = 3,
            TimeoutSeconds = 30
        };
        var result = await _sut.ExecuteSyncAsync(request);
        result.Should().NotBeNull();
        result.Status.Should().Be(OperationStatus.Completed);
        result.IsAsync.Should().BeFalse();
        result.Type.Should().Be(OperationType.DataSynchronization);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>()), Times.Once);
        _handlerMock.Verify(h => h.ExecuteAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task EnqueueAsync_CreatesOperationAsPending()
    {
        var request = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "async-payload"
        };
        var result = await _sut.EnqueueAsync(request);
        result.Should().NotBeNull();
        result.Status.Should().Be(OperationStatus.Pending);
        result.IsAsync.Should().BeTrue();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>()), Times.Once);
        _handlerMock.Verify(h => h.ExecuteAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Operation?)null);
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsOperationWhenFound()
    {
        var operationId = Guid.NewGuid();
        var operation = new Operation
        {
            Id = operationId,
            Type = OperationType.DataSynchronization,
            Status = OperationStatus.Completed,
            Payload = "data"
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(operationId, It.IsAny<CancellationToken>())).ReturnsAsync(operation);
        var result = await _sut.GetByIdAsync(operationId);
        result.Should().NotBeNull();
        result!.Id.Should().Be(operationId);
        result.Status.Should().Be(OperationStatus.Completed);
    }
    [Fact]
    public async Task RetryAsync_ThrowsWhenOperationNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Operation?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RetryAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task RetryAsync_ThrowsWhenStatusIsNotFailedOrTimedOut()
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            Status = OperationStatus.Completed,
            Type = OperationType.DataSynchronization,
            Payload = "data"
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(operation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(operation);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RetryAsync(operation.Id));
    }
    [Fact]
    public async Task RetryAsync_ThrowsWhenMaxRetriesReached()
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            Status = OperationStatus.Failed,
            Type = OperationType.DataSynchronization,
            Payload = "data",
            RetryCount = 3,
            MaxRetries = 3
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(operation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(operation);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RetryAsync(operation.Id));
    }
    [Fact]
    public async Task RetryAsync_RetriesFailedOperation()
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            Status = OperationStatus.Failed,
            Type = OperationType.DataSynchronization,
            Payload = "data",
            RetryCount = 1,
            MaxRetries = 3
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(operation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(operation);
        var result = await _sut.RetryAsync(operation.Id);
        result.Should().NotBeNull();
        result.Status.Should().Be(OperationStatus.Completed);
        result.RetryCount.Should().Be(2);
    }
    [Fact]
    public async Task CancelAsync_ThrowsWhenOperationNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Operation?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task CancelAsync_CancelsPendingOperation()
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            Status = OperationStatus.Pending,
            Type = OperationType.DataSynchronization,
            Payload = "data"
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(operation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(operation);
        await _sut.CancelAsync(operation.Id);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Operation>(o => o.Status == OperationStatus.Cancelled), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task ExecuteSyncAsync_SetsFailedStatusWhenHandlerThrows()
    {
        _handlerMock.Setup(h => h.ExecuteAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Handler error"));
        var request = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "test"
        };
        var result = await _sut.ExecuteSyncAsync(request);
        result.Status.Should().Be(OperationStatus.Failed);
        result.ErrorMessage.Should().Be("Handler error");
    }
    [Fact]
    public async Task ExecuteSyncAsync_SetsTimedOutWhenTimeoutExpires()
    {
        _handlerMock.Setup(h => h.ExecuteAsync(It.IsAny<Operation>(), It.IsAny<CancellationToken>()))
            .Returns(async (Operation op, CancellationToken ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
                return "result";
            });
        var request = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "test",
            TimeoutSeconds = 1
        };
        var result = await _sut.ExecuteSyncAsync(request);
        result.Status.Should().Be(OperationStatus.TimedOut);
    }
}