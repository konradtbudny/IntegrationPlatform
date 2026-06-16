using IntegrationPlatform.Application.Handlers;
using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOperationService, OperationService>();
        services.AddScoped<IOperationHandler, DataSynchronizationHandler>();
        services.AddScoped<IOperationHandler, FileImportHandler>();
        services.AddScoped<IOperationHandler, ReportGenerationHandler>();
        services.AddScoped<IOperationHandler, WebhookDispatchHandler>();
        return services;
    }
}