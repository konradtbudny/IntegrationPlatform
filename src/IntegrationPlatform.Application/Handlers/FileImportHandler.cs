using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Application.Handlers;
public class FileImportHandler : IOperationHandler
{
    public OperationType OperationType => OperationType.FileImport;
    public async Task<string> ExecuteAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
        return $"FileImport completed for payload: {operation.Payload}";
    }
}