using Microsoft.EntityFrameworkCore.Design;

namespace TaskList.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=tasklist.db").Options;
        return new AppDbContext(options);
    }
}
