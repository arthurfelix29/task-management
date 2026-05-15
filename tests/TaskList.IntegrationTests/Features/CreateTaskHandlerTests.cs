using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TaskList.Api.Features.Tasks.CreateTask;
using TaskList.Domain.Tasks;
using TaskList.IntegrationTests.Fixtures;

namespace TaskList.IntegrationTests.Features;

public sealed class CreateTaskHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task When_CreatingTaskWithValidTitle_Should_PersistAndReturnSuccess()
    {
        // Arrange
        await using var db = new TestDb();
        var clock = new FakeTimeProvider(Now);
        var handler = new CreateTaskHandler(db.Context, clock);
        var command = new CreateTaskCommand("Buy groceries");

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("Buy groceries");
        result.Value.IsCompleted.ShouldBeFalse();
        result.Value.CreatedAt.ShouldBe(Now);

        var persisted = await db.Context.Tasks
            .FirstOrDefaultAsync(t => t.Id == new TaskId(result.Value.Id), TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
        persisted!.Title.ShouldBe("Buy groceries");
    }
}
