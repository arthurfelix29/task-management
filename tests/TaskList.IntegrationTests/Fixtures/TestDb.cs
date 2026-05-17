using Microsoft.Data.Sqlite;
using TaskList.Infrastructure.Persistence;

namespace TaskList.IntegrationTests.Fixtures;

public sealed class TestDb : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public TestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public AppDbContext Context { get; }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
