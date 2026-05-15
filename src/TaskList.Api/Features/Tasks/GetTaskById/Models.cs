using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.GetTaskById;

public sealed record GetTaskByIdQuery(TaskId Id);
