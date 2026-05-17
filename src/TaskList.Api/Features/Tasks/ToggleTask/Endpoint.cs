using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskList.Api.Common;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.Extensions;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.ToggleTask;

public sealed class ToggleTaskEndpoint : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        Guard.Against.Null(app);

        app.MapPost(Routes.Tasks.Toggle, HandleAsync)
            .WithName(RouteNames.Tasks.Toggle)
            .WithTags("Tasks")
            .AllowAnonymous()
            .Produces<TaskResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<TaskResponse>, ProblemHttpResult>> HandleAsync(
        Guid id,
        ICommandHandler<ToggleTaskCommand, Result<TaskResponse>> handler,
        LinkGenerator links,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ToggleTaskCommand(new TaskId(id)), cancellationToken);
        if (result.IsFailure)
            return result.ToProblemDetails();

        var task = result.Value with { Links = TaskLinks.ForItem(links, httpContext, result.Value.Id) };
        return TypedResults.Ok(task);
    }
}
