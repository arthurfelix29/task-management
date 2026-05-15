using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.Mapping;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    IReadOnlyList<LinkResponse> Links)
{
    public static TaskResponse From(TaskItem task)
    {
        Guard.Against.Null(task);
        return new TaskResponse(task.Id, task.Title, task.IsCompleted, task.CreatedAt, []);
    }
}
