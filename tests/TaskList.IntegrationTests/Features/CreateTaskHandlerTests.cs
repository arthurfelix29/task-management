using Microsoft.Extensions.Time.Testing;
using TaskList.Api.Features.Tasks.CreateTask;
using TaskList.Domain.Common;

namespace TaskList.IntegrationTests.Features;

public sealed class CreateTaskHandlerTests
{
    private static readonly DateTimeOffset _now = new(2026, 5, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task When_CreatingTaskWithValidTitle_Should_PersistAndReturnSuccess()
    {
        // Arrange
        await using var db = new TestDb();
        var clock = new FakeTimeProvider(_now);
        var handler = new CreateTaskHandler(db.Context, clock);
        var command = new CreateTaskCommand("Buy groceries");

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("Buy groceries");
        result.Value.IsCompleted.ShouldBeFalse();
        result.Value.CreatedAt.ShouldBe(_now);

        var persisted = await db.Context.Tasks
            .FirstOrDefaultAsync(t => t.Id == new TaskId(result.Value.Id), TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
        persisted!.Title.ShouldBe("Buy groceries");
    }

    [Fact]
    public async Task When_CreatingTaskWithDuplicateTitle_Should_ReturnConflictFailure()
    {
        // Arrange
        await using var db = new TestDb();
        var clock = new FakeTimeProvider(_now);
        var handler = new CreateTaskHandler(db.Context, clock);

        var seed = await handler.HandleAsync(new CreateTaskCommand("Buy milk"), TestContext.Current.CancellationToken);
        seed.IsSuccess.ShouldBeTrue();

        // Act
        var duplicate = await handler.HandleAsync(new CreateTaskCommand("  BUY MILK  "), TestContext.Current.CancellationToken);

        // Assert
        duplicate.IsFailure.ShouldBeTrue();
        duplicate.Error.Type.ShouldBe(ErrorType.Conflict);
        duplicate.Error.Code.ShouldBe("task.duplicate_title");

        var count = await db.Context.Tasks.CountAsync(TestContext.Current.CancellationToken);
        count.ShouldBe(1);
    }
}
