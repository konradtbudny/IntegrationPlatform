namespace IntegrationPlatform.Domain.Enums;
public enum OperationStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    TimedOut = 5
}