using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler(AppDbContext db, TimeProvider clock)
    : ICommandHandler<CreateTaskCommand, Result<TaskResponse>>
{
    public async Task<Result<TaskResponse>> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var task = TaskItem.Create(command.Title, clock);

        db.Tasks.Add(task);
        await db.SaveChangesAsync(cancellationToken);

        return TaskResponse.From(task);
    }
}
