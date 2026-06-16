using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Contracts.Responses;
public class OperationHistoryResponse
{
    public Guid Id { get; set; }
    public OperationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}