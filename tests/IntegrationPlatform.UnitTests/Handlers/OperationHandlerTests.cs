using FluentAssertions;
using IntegrationPlatform.Application.Handlers;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.UnitTests.Handlers;
public class OperationHandlerTests
{
    [Fact]
    public async Task DataSynchronizationHandler_ReturnsExpectedResult()
    {
        var handler = new DataSynchronizationHandler();
        var operation = new Operation { Type = OperationType.DataSynchronization, Payload = "test-payload" };
        var result = await handler.ExecuteAsync(operation);
        result.Should().Contain("DataSynchronization");
        result.Should().Contain("test-payload");
    }
    [Fact]
    public async Task FileImportHandler_ReturnsExpectedResult()
    {
        var handler = new FileImportHandler();
        var operation = new Operation { Type = OperationType.FileImport, Payload = "file.csv" };
        var result = await handler.ExecuteAsync(operation);
        result.Should().Contain("FileImport");
        result.Should().Contain("file.csv");
    }
    [Fact]
    public async Task ReportGenerationHandler_ReturnsExpectedResult()
    {
        var handler = new ReportGenerationHandler();
        var operation = new Operation { Type = OperationType.ReportGeneration, Payload = "monthly-report" };
        var result = await handler.ExecuteAsync(operation);
        result.Should().Contain("ReportGeneration");
        result.Should().Contain("monthly-report");
    }
    [Fact]
    public async Task WebhookDispatchHandler_ReturnsExpectedResult()
    {
        var handler = new WebhookDispatchHandler();
        var operation = new Operation { Type = OperationType.WebhookDispatch, Payload = "https://example.com/hook" };
        var result = await handler.ExecuteAsync(operation);
        result.Should().Contain("WebhookDispatch");
        result.Should().Contain("https://example.com/hook");
    }
    [Fact]
    public void DataSynchronizationHandler_HasCorrectOperationType()
    {
        var handler = new DataSynchronizationHandler();
        handler.OperationType.Should().Be(OperationType.DataSynchronization);
    }
    [Fact]
    public void FileImportHandler_HasCorrectOperationType()
    {
        var handler = new FileImportHandler();
        handler.OperationType.Should().Be(OperationType.FileImport);
    }
    [Fact]
    public void ReportGenerationHandler_HasCorrectOperationType()
    {
        var handler = new ReportGenerationHandler();
        handler.OperationType.Should().Be(OperationType.ReportGeneration);
    }
    [Fact]
    public void WebhookDispatchHandler_HasCorrectOperationType()
    {
        var handler = new WebhookDispatchHandler();
        handler.OperationType.Should().Be(OperationType.WebhookDispatch);
    }
    [Fact]
    public async Task DataSynchronizationHandler_RespectsCancellationToken()
    {
        var handler = new DataSynchronizationHandler();
        var operation = new Operation { Type = OperationType.DataSynchronization, Payload = "payload" };
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.ExecuteAsync(operation, cts.Token));
    }
}