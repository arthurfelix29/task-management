using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using TaskList.Api.Common.Endpoints;

namespace TaskList.Api.Common.Extensions;

public static class WebApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        Guard.Against.Null(app);

        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapEndpointGroups();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
        });
    }
}
