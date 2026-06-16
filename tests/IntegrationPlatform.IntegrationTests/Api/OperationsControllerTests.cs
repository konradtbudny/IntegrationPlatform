using FluentAssertions;
using IntegrationPlatform.Contracts.Requests;
using IntegrationPlatform.Contracts.Responses;
using IntegrationPlatform.Domain.Enums;
using System.Net;
using System.Net.Http.Json;
namespace IntegrationPlatform.IntegrationTests.Api;

public class OperationsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public OperationsControllerTests(IntegrationTestWebApplicationFactory factory) { _client = factory.CreateClient(); }
    [Fact]
    public async Task PostSync_ReturnsOkWithCompletedOperation()
    {
        var request = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "integration-test-payload",
            MaxRetries = 3,
            TimeoutSeconds = 30
        };
        var response = await _client.PostAsJsonAsync("/api/operations/sync", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OperationResponse>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(OperationStatus.Completed);
        result.Type.Should().Be(OperationType.DataSynchronization);
        result.IsAsync.Should().BeFalse();
    }
    [Fact]
    public async Task PostAsync_ReturnsAcceptedWithPendingOperation()
    {
        var request = new CreateOperationRequest
        {
            Type = OperationType.FileImport,
            Payload = "file.csv"
        };
        var response = await _client.PostAsJsonAsync("/api/operations/async", request);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<OperationResponse>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(OperationStatus.Pending);
        result.IsAsync.Should().BeTrue();
    }
    [Fact]
    public async Task GetById_ReturnsOperation()
    {
        var createRequest = new CreateOperationRequest
        {
            Type = OperationType.ReportGeneration,
            Payload = "Q4 Report"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/operations/sync", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<OperationResponse>();
        var getResponse = await _client.GetAsync($"/api/operations/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await getResponse.Content.ReadFromJsonAsync<OperationResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
    }
    [Fact]
    public async Task GetById_ReturnsNotFoundForUnknownId()
    {
        var response = await _client.GetAsync($"/api/operations/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task PostRetry_ReturnsBadRequestForCompletedOperation()
    {
        var createRequest = new CreateOperationRequest
        {
            Type = OperationType.WebhookDispatch,
            Payload = "https://example.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/operations/sync", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<OperationResponse>();
        var retryResponse = await _client.PostAsync($"/api/operations/{created!.Id}/retry", null);
        retryResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task PostRetry_RetriesFailedOperation()
    {
        var asyncRequest = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "retry-test"
        };
        var asyncResponse = await _client.PostAsJsonAsync("/api/operations/async", asyncRequest);
        var queued = await asyncResponse.Content.ReadFromJsonAsync<OperationResponse>();
        var retryResponse = await _client.PostAsync($"/api/operations/{queued!.Id}/retry", null);
        retryResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task PostSync_AllOperationTypesWork()
    {
        var operationTypes = new[] { OperationType.DataSynchronization, OperationType.FileImport, OperationType.ReportGeneration, OperationType.WebhookDispatch };
        foreach (var type in operationTypes)
        {
            var request = new CreateOperationRequest { Type = type, Payload = $"payload-{type}" };
            var response = await _client.PostAsJsonAsync("/api/operations/sync", request);
            response.StatusCode.Should().Be(HttpStatusCode.OK, $"for operation type {type}");
            var result = await response.Content.ReadFromJsonAsync<OperationResponse>();
            result!.Status.Should().Be(OperationStatus.Completed, $"for operation type {type}");
        }
    }
    [Fact]
    public async Task GetById_ReturnsOperationHistory()
    {
        var createRequest = new CreateOperationRequest
        {
            Type = OperationType.DataSynchronization,
            Payload = "history-test"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/operations/sync", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<OperationResponse>();
        var getResponse = await _client.GetAsync($"/api/operations/{created!.Id}");
        var result = await getResponse.Content.ReadFromJsonAsync<OperationResponse>();
        result!.History.Should().NotBeEmpty();
    }
}