using TaskList.Domain.Common;

namespace TaskList.Domain.Tasks;

public static class TaskErrors
{
    public static Error NotFound(TaskId id) =>
        Error.NotFound("task.not_found", $"Task '{id}' was not found.");
}
