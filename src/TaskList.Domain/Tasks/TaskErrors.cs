using TaskList.Domain.Common;

namespace TaskList.Domain.Tasks;

public static class TaskErrors
{
    public static DomainError NotFound(TaskId id)
        => DomainError.NotFound("task.not_found", $"Task '{id}' was not found.");

    public static DomainError DuplicateTitle(string title)
        => DomainError.Conflict("task.duplicate_title", $"A task with the title '{title}' already exists.");
}
