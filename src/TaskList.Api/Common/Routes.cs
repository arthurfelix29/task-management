namespace TaskList.Api.Common;

public static class Routes
{
    public static class Tasks
    {
        public const string Create = "/api/v1/tasks";
        public const string List = "/api/v1/tasks";
        public const string GetById = "/api/v1/tasks/{id:guid}";
        public const string Toggle = "/api/v1/tasks/{id:guid}/toggle";
        public const string Delete = "/api/v1/tasks/{id:guid}";
    }
}
