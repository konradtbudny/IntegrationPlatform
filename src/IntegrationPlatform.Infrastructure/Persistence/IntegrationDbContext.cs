using IntegrationPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace IntegrationPlatform.Infrastructure.Persistence;
public class IntegrationDbContext : DbContext
{
    public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options) { }
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<OperationHistory> OperationHistories => Set<OperationHistory>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Payload).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.Result).HasMaxLength(8000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.HasMany(e => e.History).WithOne(h => h.Operation).HasForeignKey(h => h.OperationId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<OperationHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(2000);
        });
    }
}