using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;
using TaskList.Infrastructure.Persistence.Seeding;

namespace TaskList.Infrastructure;

public static class DependencyInjection
{
    private const int SeedTaskCount = 10;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.TryAddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=tasklist.db";

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);

            options.UseAsyncSeeding(async (context, _, ct) =>
            {
                if (await context.Set<TaskItem>().AnyAsync(ct))
                {
                    return;
                }

                var clock = sp.GetRequiredService<TimeProvider>();
                var seed = TaskSeeder.Generate(clock, SeedTaskCount);
                await context.Set<TaskItem>().AddRangeAsync(seed, ct);
                await context.SaveChangesAsync(ct);
            });

            options.UseSeeding((context, _) =>
            {
                if (context.Set<TaskItem>().Any())
                {
                    return;
                }

                var clock = sp.GetRequiredService<TimeProvider>();
                var seed = TaskSeeder.Generate(clock, SeedTaskCount);
                context.Set<TaskItem>().AddRange(seed);
                context.SaveChanges();
            });
        });

        return services;
    }
}
