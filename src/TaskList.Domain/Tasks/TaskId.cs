namespace TaskList.Domain.Tasks;

public readonly record struct TaskId(Guid Value)
{
    public static TaskId New() => new(Guid.CreateVersion7());

    public static implicit operator Guid(TaskId id) => id.Value;

    public Guid ToGuid() => Value;

    public override string ToString() => Value.ToString();
}
