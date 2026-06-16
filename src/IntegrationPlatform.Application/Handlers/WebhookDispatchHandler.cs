using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Application.Handlers;
public class WebhookDispatchHandler : IOperationHandler
{
    public OperationType OperationType => OperationType.WebhookDispatch;
    public async Task<string> ExecuteAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        return $"WebhookDispatch completed for payload: {operation.Payload}";
    }
}