using TaskList.Api.Features.Tasks.Hateoas;

namespace TaskList.Api.Features.Tasks.ListTasks;

public sealed record ListTasksResponse(IReadOnlyList<TaskResponse> Data, IReadOnlyList<LinkResponse> Links);
