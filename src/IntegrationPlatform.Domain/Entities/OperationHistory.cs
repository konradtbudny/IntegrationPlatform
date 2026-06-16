using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Domain.Entities;
public class OperationHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OperationId { get; set; }
    public OperationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Operation Operation { get; set; } = null!;
}