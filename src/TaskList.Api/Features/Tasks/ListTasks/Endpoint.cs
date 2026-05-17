using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskList.Api.Common;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.Extensions;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;

namespace TaskList.Api.Features.Tasks.ListTasks;

public sealed class ListTasksEndpoint : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        Guard.Against.Null(app);

        app.MapGet(Routes.Tasks.List, HandleAsync)
            .WithName(RouteNames.Tasks.List)
            .WithTags("Tasks")
            .AllowAnonymous()
            .Produces<ListTasksResponse>();
    }

    private static async Task<Results<Ok<ListTasksResponse>, ProblemHttpResult>> HandleAsync(
        IQueryHandler<ListTasksQuery, Result<IReadOnlyList<TaskResponse>>> handler,
        LinkGenerator links,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ListTasksQuery(), cancellationToken);
        if (result.IsFailure)
            return result.ToProblemDetails();

        var tasksWithLinks = result.Value
            .Select(t => t with { Links = TaskLinks.ForItem(links, httpContext, t.Id) })
            .ToList();

        var response = new ListTasksResponse(
            tasksWithLinks,
            tasksWithLinks.Count,
            TaskLinks.ForCollection(links, httpContext));

        return TypedResults.Ok(response);
    }
}
