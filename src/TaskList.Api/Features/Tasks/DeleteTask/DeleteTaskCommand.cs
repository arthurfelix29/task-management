using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(TaskId Id);
