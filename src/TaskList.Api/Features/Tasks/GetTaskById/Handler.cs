using Microsoft.EntityFrameworkCore;
using TaskList.Api.Features.Tasks.Mapping;
using TaskList.Application.Abstractions;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.Infrastructure.Persistence;

namespace TaskList.Api.Features.Tasks.GetTaskById;

public sealed class GetTaskByIdHandler(AppDbContext db)
    : IQueryHandler<GetTaskByIdQuery, Result<TaskResponse>>
{
    public async Task<Result<TaskResponse>> HandleAsync(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var task = await db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.Id, cancellationToken)
            .ConfigureAwait(false);

        if (task is null)
        {
            return TaskErrors.NotFound(query.Id);
        }

        return TaskResponse.From(task);
    }
}
