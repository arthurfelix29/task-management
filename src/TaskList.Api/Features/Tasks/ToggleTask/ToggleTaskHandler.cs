using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Hateoas;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.ToggleTask;

public sealed class ToggleTaskHandler(AppDbContext db) : ICommandHandler<ToggleTaskCommand, Result<TaskResponse>>
{
    public async Task<Result<TaskResponse>> HandleAsync(ToggleTaskCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var task = await db.Tasks.AsTracking().FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (task is null)
            return TaskErrors.NotFound(command.Id);

        task.Toggle();
        await db.SaveChangesAsync(cancellationToken);

        return TaskResponse.From(task);
    }
}
