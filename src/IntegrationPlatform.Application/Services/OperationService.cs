using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Application.Mappings;
using IntegrationPlatform.Contracts.Requests;
using IntegrationPlatform.Contracts.Responses;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
using IntegrationPlatform.Domain.Interfaces;
using Microsoft.Extensions.Logging;
namespace IntegrationPlatform.Application.Services;

public class OperationService : IOperationService
{
    private readonly IOperationRepository _repository;
    private readonly IEnumerable<IOperationHandler> _handlers;
    private readonly ILogger<OperationService> _logger;
    public OperationService(IOperationRepository repository, IEnumerable<IOperationHandler> handlers, ILogger<OperationService> logger)
    {
        _repository = repository;
        _handlers = handlers;
        _logger = logger;
    }
    public async Task<OperationResponse> ExecuteSyncAsync(CreateOperationRequest request, CancellationToken cancellationToken = default)
    {
        var operation = CreateOperation(request, isAsync: false);
        await _repository.AddAsync(operation, cancellationToken);
        await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Pending, "Operation created"), cancellationToken);
        await ExecuteOperationAsync(operation, cancellationToken);
        return OperationMapper.ToResponse(operation);
    }
    public async Task<OperationResponse> EnqueueAsync(CreateOperationRequest request, CancellationToken cancellationToken = default)
    {
        var operation = CreateOperation(request, isAsync: true);
        await _repository.AddAsync(operation, cancellationToken);
        await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Pending, "Operation enqueued for async processing"), cancellationToken);
        _logger.LogInformation("Operation {OperationId} enqueued for async processing", operation.Id);
        return OperationMapper.ToResponse(operation);
    }
    public async Task<OperationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var operation = await _repository.GetByIdAsync(id, cancellationToken);
        return operation is null ? null : OperationMapper.ToResponse(operation);
    }
    public async Task<OperationResponse> RetryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var operation = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Operation {id} not found");
        if (operation.Status is not (OperationStatus.Failed or OperationStatus.TimedOut)) { throw new InvalidOperationException($"Operation {id} cannot be retried in status {operation.Status}"); }
        if (operation.RetryCount >= operation.MaxRetries) { throw new InvalidOperationException($"Operation {id} has reached maximum retries ({operation.MaxRetries})"); }
        operation.RetryCount++;
        operation.Status = OperationStatus.Pending;
        operation.ErrorMessage = null;
        await _repository.UpdateAsync(operation, cancellationToken);
        await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Pending, $"Retry attempt {operation.RetryCount}"), cancellationToken);
        await ExecuteOperationAsync(operation, cancellationToken);
        return OperationMapper.ToResponse(operation);
    }
    public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var operation = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Operation {id} not found");
        if (operation.Status is not (OperationStatus.Pending or OperationStatus.Running)) { throw new InvalidOperationException($"Operation {id} cannot be cancelled in status {operation.Status}"); }
        operation.Status = OperationStatus.Cancelled;
        operation.CompletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(operation, cancellationToken);
        await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Cancelled, "Operation cancelled"), cancellationToken);
    }
    public async Task ExecuteOperationAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        var handler = _handlers.FirstOrDefault(h => h.OperationType == operation.Type) ?? throw new InvalidOperationException($"No handler registered for operation type {operation.Type}");
        operation.Status = OperationStatus.Running;
        operation.StartedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(operation, cancellationToken);
        await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Running, "Operation started"), cancellationToken);
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(operation.TimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        try
        {
            _logger.LogInformation("Executing operation {OperationId} of type {Type}", operation.Id, operation.Type);
            var result = await handler.ExecuteAsync(operation, linkedCts.Token);
            operation.Status = OperationStatus.Completed;
            operation.Result = result;
            operation.CompletedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(operation, cancellationToken);
            await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Completed, "Operation completed successfully"), cancellationToken);
            _logger.LogInformation("Operation {OperationId} completed successfully", operation.Id);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            operation.Status = OperationStatus.TimedOut;
            operation.ErrorMessage = $"Operation timed out after {operation.TimeoutSeconds} seconds";
            operation.CompletedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(operation, cancellationToken);
            await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.TimedOut, operation.ErrorMessage), cancellationToken);
            _logger.LogWarning("Operation {OperationId} timed out", operation.Id);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            operation.Status = OperationStatus.Failed;
            operation.ErrorMessage = ex.Message;
            operation.CompletedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(operation, cancellationToken);
            await _repository.AddHistoryAsync(CreateHistory(operation, OperationStatus.Failed, $"Operation failed: {ex.Message}"), cancellationToken);
            _logger.LogError(ex, "Operation {OperationId} failed", operation.Id);
        }
    }
    private static Operation CreateOperation(CreateOperationRequest request, bool isAsync)
    {
        return new Operation
        {
            Type = request.Type,
            Payload = request.Payload,
            IsAsync = isAsync,
            MaxRetries = request.MaxRetries,
            TimeoutSeconds = request.TimeoutSeconds
        };
    }
    private static OperationHistory CreateHistory(Operation operation, OperationStatus status, string? message)
    {
        return new OperationHistory
        {
            OperationId = operation.Id,
            Status = status,
            Message = message
        };
    }
}