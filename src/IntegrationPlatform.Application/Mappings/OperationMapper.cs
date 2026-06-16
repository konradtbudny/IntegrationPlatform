using IntegrationPlatform.Contracts.Responses;
using IntegrationPlatform.Domain.Entities;
namespace IntegrationPlatform.Application.Mappings;
public static class OperationMapper
{
    public static OperationResponse ToResponse(Operation operation)
    {
        return new OperationResponse
        {
            Id = operation.Id,
            Type = operation.Type,
            Status = operation.Status,
            Payload = operation.Payload,
            Result = operation.Result,
            ErrorMessage = operation.ErrorMessage,
            IsAsync = operation.IsAsync,
            RetryCount = operation.RetryCount,
            MaxRetries = operation.MaxRetries,
            TimeoutSeconds = operation.TimeoutSeconds,
            CreatedAt = operation.CreatedAt,
            StartedAt = operation.StartedAt,
            CompletedAt = operation.CompletedAt,
            History = operation.History
                .OrderByDescending(h => h.Timestamp)
                .Select(h => new OperationHistoryResponse
                {
                    Id = h.Id,
                    Status = h.Status,
                    Message = h.Message,
                    Timestamp = h.Timestamp
                })
                .ToList()
        };
    }
}