using Ardalis.GuardClauses;
using TaskList.Domain.Common;

namespace TaskList.Domain.Tasks;

public sealed class TaskItem : Entity<TaskId>
{
    public string Title { get; private init; } = string.Empty;

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private init; }

    private TaskItem() { }

    public static TaskItem Create(string title, TimeProvider clock)
    {
        Guard.Against.NullOrWhiteSpace(title);
        Guard.Against.Null(clock);

        return new TaskItem
        {
            Id = TaskId.New(),
            Title = title,
            CreatedAt = clock.GetUtcNow()
        };
    }

    public void Toggle() => IsCompleted = !IsCompleted;
}
