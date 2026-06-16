using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Domain.Entities;
public class Operation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public OperationType Type { get; set; }
    public OperationStatus Status { get; set; } = OperationStatus.Pending;
    public string Payload { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAsync { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ICollection<OperationHistory> History { get; set; } = new List<OperationHistory>();
}