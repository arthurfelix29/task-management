using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Common.Hosting;

internal sealed class DatabaseMigrationService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = Guard.Against.Null(serviceProvider);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
