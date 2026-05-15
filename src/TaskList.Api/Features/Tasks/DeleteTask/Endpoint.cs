using Microsoft.AspNetCore.Http.HttpResults;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.Extensions;
using TaskList.Api.Common.Routes;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.DeleteTask;

public sealed class DeleteTaskEndpoint : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapDelete(RouteConsts.TaskByIdRoute, HandleAsync)
            .WithName(RouteConsts.Names.DeleteTask)
            .WithTags("Tasks")
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        Guid id,
        ICommandHandler<DeleteTaskCommand, Result> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteTaskCommand(new TaskId(id)), cancellationToken).ConfigureAwait(false);
        return result.IsFailure
            ? result.ToProblemDetails()
            : TypedResults.NoContent();
    }
}
