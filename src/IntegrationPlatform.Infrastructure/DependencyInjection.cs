using IntegrationPlatform.Domain.Interfaces;
using IntegrationPlatform.Infrastructure.Persistence;
using IntegrationPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace IntegrationPlatform.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IntegrationDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IOperationRepository, OperationRepository>();
        return services;
    }
}