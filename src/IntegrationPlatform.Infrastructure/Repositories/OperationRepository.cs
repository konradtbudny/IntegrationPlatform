using IntegrationPlatform.Domain.Entities;
using IntegrationPlatform.Domain.Enums;
using IntegrationPlatform.Domain.Interfaces;
using IntegrationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace IntegrationPlatform.Infrastructure.Repositories;
public class OperationRepository : IOperationRepository
{
    private readonly IntegrationDbContext _context;
    public OperationRepository(IntegrationDbContext context) { _context = context; }
    public async Task<Operation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _context.Operations.Include(o => o.History).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    public async Task<IReadOnlyList<Operation>> GetPendingAsync(CancellationToken cancellationToken = default) => await _context.Operations.Where(o => o.Status == OperationStatus.Pending && o.IsAsync).Include(o => o.History).ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<Operation>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Operations.Include(o => o.History).OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);
    public async Task AddAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await _context.Operations.AddAsync(operation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        _context.Operations.Update(operation);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task AddHistoryAsync(OperationHistory history, CancellationToken cancellationToken = default)
    {
        await _context.OperationHistories.AddAsync(history, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}