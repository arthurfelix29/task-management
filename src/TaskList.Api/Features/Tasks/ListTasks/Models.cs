using TaskList.Api.Features.Tasks.Mapping;

namespace TaskList.Api.Features.Tasks.ListTasks;

public sealed record ListTasksQuery;

public sealed record ListTasksResponse(IReadOnlyList<TaskResponse> Data, int Count, IReadOnlyList<LinkResponse> Links);
