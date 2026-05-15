using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.ToggleTask;

public sealed class ToggleTaskHandler(AppDbContext db)
    : ICommandHandler<ToggleTaskCommand, Result<TaskResponse>>
{
    public async Task<Result<TaskResponse>> HandleAsync(ToggleTaskCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken)
            .ConfigureAwait(false);

        if (task is null)
        {
            return TaskErrors.NotFound(command.Id);
        }

        task.Toggle();
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return TaskResponse.From(task);
    }
}
