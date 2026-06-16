using IntegrationPlatform.Domain.Entities;
namespace IntegrationPlatform.Domain.Interfaces;
public interface IOperationRepository
{
    Task<Operation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Operation>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Operation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Operation operation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Operation operation, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(OperationHistory history, CancellationToken cancellationToken = default);
}