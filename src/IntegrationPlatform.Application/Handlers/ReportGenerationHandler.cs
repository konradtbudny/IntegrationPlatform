using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Application.Handlers;
public class ReportGenerationHandler : IOperationHandler
{
    public OperationType OperationType => OperationType.ReportGeneration;
    public async Task<string> ExecuteAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(800), cancellationToken);
        return $"ReportGeneration completed for payload: {operation.Payload}";
    }
}