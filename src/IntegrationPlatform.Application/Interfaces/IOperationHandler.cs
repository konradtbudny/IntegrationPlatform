using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Application.Interfaces;
public interface IOperationHandler
{
    OperationType OperationType { get; }
    Task<string> ExecuteAsync(Operation operation, CancellationToken cancellationToken = default);
}