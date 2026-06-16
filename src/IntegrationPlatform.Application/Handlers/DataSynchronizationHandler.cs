using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Application.Handlers;
public class DataSynchronizationHandler : IOperationHandler
{
    public OperationType OperationType => OperationType.DataSynchronization;
    public async Task<string> ExecuteAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        return $"DataSynchronization completed for payload: {operation.Payload}";
    }
}