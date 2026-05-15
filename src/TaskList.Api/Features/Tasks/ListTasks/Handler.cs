using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.ListTasks;

public sealed class ListTasksHandler(AppDbContext db)
    : IQueryHandler<ListTasksQuery, Result<IReadOnlyList<TaskResponse>>>
{
    public async Task<Result<IReadOnlyList<TaskResponse>>> HandleAsync(
        ListTasksQuery query,
        CancellationToken cancellationToken)
    {
        var tasks = await db.Tasks
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var ordered = tasks
            .OrderByDescending(t => t.CreatedAt)
            .Select(TaskResponse.From)
            .ToList();

        return Result<IReadOnlyList<TaskResponse>>.Success(ordered);
    }
}
