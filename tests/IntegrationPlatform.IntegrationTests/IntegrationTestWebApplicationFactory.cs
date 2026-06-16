using IntegrationPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace IntegrationPlatform.IntegrationTests;
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "IntegrationTestDb_" + Guid.NewGuid();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services.Where(d => d.ServiceType == typeof(DbContextOptions<IntegrationDbContext>) || d.ServiceType == typeof(IntegrationDbContext)).ToList();
            foreach (var descriptor in descriptorsToRemove) { services.Remove(descriptor); }
            services.AddDbContext<IntegrationDbContext>(options => options.UseInMemoryDatabase(_dbName));
        });
        builder.UseEnvironment("Testing");
    }
}