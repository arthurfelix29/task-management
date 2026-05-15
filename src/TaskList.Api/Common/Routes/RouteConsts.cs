namespace TaskList.Api.Common.Routes;

public static class RouteConsts
{
    public const string ApiV1 = "/api/v1";
    public const string TasksRoute = $"{ApiV1}/tasks";
    public const string TaskByIdRoute = $"{TasksRoute}/{{id:guid}}";
    public const string ToggleTaskRoute = $"{TaskByIdRoute}/toggle";

    public static class Names
    {
        public const string CreateTask = "Tasks.Create";
        public const string ListTasks = "Tasks.List";
        public const string GetTaskById = "Tasks.GetById";
        public const string ToggleTask = "Tasks.Toggle";
        public const string DeleteTask = "Tasks.Delete";
    }
}
