using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Hateoas;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler(AppDbContext db, TimeProvider clock) : ICommandHandler<CreateTaskCommand, Result<TaskResponse>>
{
    public async Task<Result<TaskResponse>> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var normalizedTitle = command.Title.Trim();

        var existingTitles = await db.Tasks.Select(t => t.Title).ToListAsync(cancellationToken);
        var duplicateExists = existingTitles.Exists(t => string.Equals(t.Trim(), normalizedTitle, StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
            return TaskErrors.DuplicateTitle(normalizedTitle);

        var task = TaskItem.Create(normalizedTitle, clock);

        db.Tasks.Add(task);
        await db.SaveChangesAsync(cancellationToken);

        return TaskResponse.From(task);
    }
}
