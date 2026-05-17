namespace TaskList.Domain.Common;

public abstract class Entity<TId> where TId : struct
{
    public TId Id { get; protected init; }
}
