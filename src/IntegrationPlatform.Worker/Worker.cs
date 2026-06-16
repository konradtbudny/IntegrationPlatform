using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Domain.Interfaces;
namespace IntegrationPlatform.Worker;
public class OperationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OperationWorker> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    public OperationWorker(IServiceScopeFactory scopeFactory, ILogger<OperationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OperationWorker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessPendingOperationsAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException) { _logger.LogError(ex, "Error in OperationWorker loop"); }
            await Task.Delay(PollingInterval, stoppingToken);
        }
        _logger.LogInformation("OperationWorker stopped");
    }
    private async Task ProcessPendingOperationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOperationRepository>();
        var operationService = scope.ServiceProvider.GetRequiredService<IOperationService>();
        var pendingOperations = await repository.GetPendingAsync(stoppingToken);
        if (pendingOperations.Count == 0) { return; }
        _logger.LogInformation("Found {Count} pending async operation(s) to process", pendingOperations.Count);
        foreach (var operation in pendingOperations)
        {
            if (stoppingToken.IsCancellationRequested) { break; }
            try
            {
                _logger.LogInformation("Processing async operation {OperationId}", operation.Id);
                await operationService.ExecuteOperationAsync(operation, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException) { _logger.LogError(ex, "Failed to process operation {OperationId}", operation.Id); }
        }
    }
}