using Bogus;

namespace TaskList.IntegrationTests.Fixtures;

public static class TaskFaker
{
    private static readonly Faker _faker = new();

    public static TaskItem ATask(TimeProvider clock)
        => TaskItem.Create(_faker.Lorem.Sentence(wordCount: 3), clock);
}
