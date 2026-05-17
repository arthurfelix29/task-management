namespace TaskList.Infrastructure.Persistence.Seeding;

internal static class TaskSeeder
{
    private static readonly string[] _seedTitles =
    [
        "Review pull requests from Q2 sprint backlog",
        "Pick up Maria from school at 4 PM",
        "Schedule dentist appointment for next month",
        "Refactor authentication middleware before release",
        "Buy groceries for the week",
        "Update LinkedIn profile with current role",
        "Prepare slides for Monday's architecture review",
        "Call mom about the weekend visit",
        "Run database backup script and verify",
        "Read chapter 4 of Designing Data-Intensive Applications",
    ];

    private static readonly HashSet<int> _preCompletedIndexes = [1, 4, 7];

    public static IReadOnlyList<TaskItem> Generate(TimeProvider clock)
    {
        Guard.Against.Null(clock);

        var tasks = new List<TaskItem>(_seedTitles.Length);
        for (var i = 0; i < _seedTitles.Length; i++)
        {
            var task = TaskItem.Create(_seedTitles[i], clock);
            if (_preCompletedIndexes.Contains(i))
                task.Toggle();

            tasks.Add(task);
        }

        return tasks;
    }
}
