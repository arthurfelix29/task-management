using Ardalis.GuardClauses;
using TaskList.Api.Common;

namespace TaskList.Api.Features.Tasks.Mapping;

internal static class TaskLinks
{
    public static IReadOnlyList<LinkResponse> ForItem(LinkGenerator links, HttpContext httpContext, Guid id)
    {
        Guard.Against.Null(links);
        Guard.Against.Null(httpContext);

        return
        [
            new LinkResponse(Build(links, httpContext, RouteNames.Tasks.GetById, new { id }), "self", HttpMethods.Get),
            new LinkResponse(Build(links, httpContext, RouteNames.Tasks.Toggle, new { id }), "toggle", HttpMethods.Post),
            new LinkResponse(Build(links, httpContext, RouteNames.Tasks.Delete, new { id }), "delete", HttpMethods.Delete),
        ];
    }

    public static IReadOnlyList<LinkResponse> ForCollection(LinkGenerator links, HttpContext httpContext)
    {
        Guard.Against.Null(links);
        Guard.Against.Null(httpContext);

        return
        [
            new LinkResponse(Build(links, httpContext, RouteNames.Tasks.List, values: null), "self", HttpMethods.Get),
            new LinkResponse(Build(links, httpContext, RouteNames.Tasks.Create, values: null), "create", HttpMethods.Post),
        ];
    }

    private static string Build(LinkGenerator links, HttpContext httpContext, string routeName, object? values)
    {
        var uri = links.GetUriByName(httpContext, routeName, values);
        return uri ?? throw new InvalidOperationException($"Route '{routeName}' is not registered.");
    }
}
