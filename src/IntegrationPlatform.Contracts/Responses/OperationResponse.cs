using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Contracts.Responses;
public class OperationResponse
{
    public Guid Id { get; set; }
    public OperationType Type { get; set; }
    public OperationStatus Status { get; set; }
    public string Payload { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAsync { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
    public int TimeoutSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public IReadOnlyList<OperationHistoryResponse> History { get; set; } = [];
}