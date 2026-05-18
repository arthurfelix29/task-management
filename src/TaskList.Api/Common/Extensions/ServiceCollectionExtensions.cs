using Ardalis.GuardClauses;
using FluentValidation;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.ExceptionHandling;
using TaskList.Api.Common.Hosting;
using TaskList.Application.Abstractions;
using TaskList.Infrastructure;

namespace TaskList.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddInfrastructure(configuration);
        services.AddEndpointGroups();
        services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tasklist.db";

        services.AddHealthChecks().AddSqlite(connectionString, name: "database", tags: ["ready"]);

        services.AddHostedService<DatabaseMigrationService>();

        return services;
    }
}
