using Bogus;
using TaskList.Domain.Tasks;

namespace TaskList.IntegrationTests.Fixtures;

public static class TaskFaker
{
    private static readonly Faker Faker = new();

    public static TaskItem ATask(TimeProvider clock) =>
        TaskItem.Create(Faker.Lorem.Sentence(wordCount: 3), clock);
}
