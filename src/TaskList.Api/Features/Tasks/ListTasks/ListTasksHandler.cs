using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Hateoas;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.ListTasks;

public sealed class ListTasksHandler(AppDbContext db) : IQueryHandler<ListTasksQuery, Result<IReadOnlyList<TaskResponse>>>
{
    public async Task<Result<IReadOnlyList<TaskResponse>>> HandleAsync(ListTasksQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        var tasks = await db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<TaskResponse>>(tasks.ConvertAll(TaskResponse.From));
    }
}
