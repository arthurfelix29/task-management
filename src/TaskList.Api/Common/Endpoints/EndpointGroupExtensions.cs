namespace TaskList.Api.Common.Endpoints;

public static class EndpointGroupExtensions
{
    public static IServiceCollection AddEndpointGroups(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var groups = typeof(EndpointGroupExtensions).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpointGroup).IsAssignableFrom(t));

        foreach (var group in groups)
        {
            services.AddSingleton(typeof(IEndpointGroup), group);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpointGroups(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        foreach (var group in app.ServiceProvider.GetServices<IEndpointGroup>())
        {
            group.Map(app);
        }

        return app;
    }
}
