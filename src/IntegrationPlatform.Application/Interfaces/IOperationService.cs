using IntegrationPlatform.Contracts.Requests;
using IntegrationPlatform.Contracts.Responses;
using IntegrationPlatform.Domain.Entities;
namespace IntegrationPlatform.Application.Interfaces;
public interface IOperationService
{
    Task<OperationResponse> ExecuteSyncAsync(CreateOperationRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse> EnqueueAsync(CreateOperationRequest request, CancellationToken cancellationToken = default);
    Task<OperationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OperationResponse> RetryAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExecuteOperationAsync(Operation operation, CancellationToken cancellationToken = default);
}