using IntegrationPlatform.Domain.Enums;
namespace IntegrationPlatform.Contracts.Requests;
public class CreateOperationRequest
{
    public OperationType Type { get; set; }
    public string Payload { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
}