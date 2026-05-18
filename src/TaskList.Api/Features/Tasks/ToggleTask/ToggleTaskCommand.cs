using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.ToggleTask;

public sealed record ToggleTaskCommand(TaskId Id);
