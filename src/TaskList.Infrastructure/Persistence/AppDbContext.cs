using Microsoft.EntityFrameworkCore;
using TaskList.Domain.Tasks;

namespace TaskList.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Guard.Against.Null(optionsBuilder);

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
