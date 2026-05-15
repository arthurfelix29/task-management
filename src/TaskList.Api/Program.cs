using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.Middleware;
using TaskList.Application.Abstractions;
using TaskList.Infrastructure;
using TaskList.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointGroups();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=tasklist.db";

builder.Services.AddHealthChecks()
    .AddSqlite(connectionString, name: "database", tags: ["ready"]);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

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

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
