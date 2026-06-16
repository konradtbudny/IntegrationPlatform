using IntegrationPlatform.Application;
using IntegrationPlatform.Infrastructure;
using IntegrationPlatform.Infrastructure.Persistence;
using IntegrationPlatform.Worker;
using Microsoft.EntityFrameworkCore;
var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=integration-worker.db";
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddHostedService<OperationWorker>();
var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
    db.Database.Migrate();
}
host.Run();