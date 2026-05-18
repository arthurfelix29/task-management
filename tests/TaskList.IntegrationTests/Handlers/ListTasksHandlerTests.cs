using Microsoft.Extensions.Time.Testing;
using TaskList.Api.Features.Tasks.ListTasks;

namespace TaskList.IntegrationTests.Handlers;

public sealed class ListTasksHandlerTests
{
    [Fact]
    public async Task When_ListingTasksWithEmptyDatabase_Should_ReturnEmptyCollection()
    {
        // Arrange
        await using var db = new TestDb();
        var handler = new ListTasksHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new ListTasksQuery(), TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task When_ListingTasks_Should_ReturnInDescendingCreatedAtOrder()
    {
        // Arrange
        await using var db = new TestDb();
        var oldest = new FakeTimeProvider(new DateTimeOffset(2026, 5, 13, 9, 0, 0, TimeSpan.Zero));
        var middle = new FakeTimeProvider(new DateTimeOffset(2026, 5, 14, 9, 0, 0, TimeSpan.Zero));
        var newest = new FakeTimeProvider(new DateTimeOffset(2026, 5, 15, 9, 0, 0, TimeSpan.Zero));

        db.Context.Tasks.AddRange(
            TaskItem.Create("Oldest", oldest),
            TaskItem.Create("Newest", newest),
            TaskItem.Create("Middle", middle));
        await db.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new ListTasksHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new ListTasksQuery(), TestContext.Current.CancellationToken);

        // Assert
        result.Value.Select(t => t.Title).ShouldBe(["Newest", "Middle", "Oldest"]);
    }
}
