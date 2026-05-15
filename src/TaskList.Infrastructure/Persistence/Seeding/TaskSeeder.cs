using Bogus;
using TaskList.Domain.Tasks;

namespace TaskList.Infrastructure.Persistence.Seeding;

internal static class TaskSeeder
{
    private const int SeedRandomizerSeed = 42;

    public static IReadOnlyList<TaskItem> Generate(TimeProvider clock, int count)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        Randomizer.Seed = new Random(SeedRandomizerSeed);
        var faker = new Faker("en");
        var tasks = new List<TaskItem>(count);

        for (var i = 0; i < count; i++)
        {
            var task = TaskItem.Create(faker.Hacker.Phrase(), clock);
            if (faker.Random.Bool())
            {
                task.Toggle();
            }

            tasks.Add(task);
        }

        return tasks;
    }
}
