using Microsoft.EntityFrameworkCore;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.DeleteTask;

public sealed class DeleteTaskHandler(AppDbContext db) : ICommandHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var deleted = await db.Tasks
            .Where(t => t.Id == command.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted == 0
            ? TaskErrors.NotFound(command.Id)
            : Result.Success();
    }
}
