using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskList.Api.Common;
using TaskList.Api.Common.Endpoints;
using TaskList.Api.Common.Extensions;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;

namespace TaskList.Api.Features.Tasks.CreateTask;

public sealed class CreateTaskEndpoint : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        Guard.Against.Null(app);

        app.MapPost(Routes.Tasks.Create, HandleAsync)
            .WithName(RouteNames.Tasks.Create)
            .WithTags("Tasks")
            .AllowAnonymous()
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<Results<Created<TaskResponse>, ProblemHttpResult>> HandleAsync(
        CreateTaskCommand command,
        IValidator<CreateTaskCommand> validator,
        ICommandHandler<CreateTaskCommand, Result<TaskResponse>> handler,
        LinkGenerator links,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.Problem(new HttpValidationProblemDetails(validation.ToDictionary())
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation failed",
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-422",
            });
        }

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.ToProblemDetails();

        var task = result.Value with { Links = TaskLinks.ForItem(links, httpContext, result.Value.Id) };
        var location = links.GetUriByName(httpContext, RouteNames.Tasks.GetById, new { id = task.Id });

        return TypedResults.Created(location, task);
    }
}
