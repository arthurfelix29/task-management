using Serilog;
using TaskList.Api.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
