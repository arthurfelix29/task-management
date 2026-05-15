using TaskList.Api.Common.Routes;

namespace TaskList.Api.Features.Tasks.Mapping;

internal static class TaskLinks
{
    public static IReadOnlyList<LinkResponse> ForItem(LinkGenerator links, HttpContext httpContext, Guid id)
    {
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(httpContext);

        return
        [
            new LinkResponse(Build(links, httpContext, RouteConsts.Names.GetTaskById, new { id }), "self", HttpMethods.Get),
            new LinkResponse(Build(links, httpContext, RouteConsts.Names.ToggleTask, new { id }), "toggle", HttpMethods.Post),
            new LinkResponse(Build(links, httpContext, RouteConsts.Names.DeleteTask, new { id }), "delete", HttpMethods.Delete),
        ];
    }

    public static IReadOnlyList<LinkResponse> ForCollection(LinkGenerator links, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(httpContext);

        return
        [
            new LinkResponse(Build(links, httpContext, RouteConsts.Names.ListTasks, null), "self", HttpMethods.Get),
            new LinkResponse(Build(links, httpContext, RouteConsts.Names.CreateTask, null), "create", HttpMethods.Post),
        ];
    }

    private static string Build(LinkGenerator links, HttpContext httpContext, string routeName, object? values)
    {
        var uri = links.GetUriByName(httpContext, routeName, values);
        return uri ?? throw new InvalidOperationException($"Route '{routeName}' is not registered.");
    }
}
