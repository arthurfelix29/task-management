using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskList.Infrastructure.Persistence;

namespace TaskList.IntegrationTests.Fixtures;

public sealed class TaskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public async ValueTask InitializeAsync()
        => await _connection.OpenAsync(TestContext.Current.CancellationToken);

    public override async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }

    public async Task<TResult> WithDbAsync<TResult>(Func<AppDbContext, Task<TResult>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await action(db);
    }

    public async Task WithDbAsync(Func<AppDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }
}
