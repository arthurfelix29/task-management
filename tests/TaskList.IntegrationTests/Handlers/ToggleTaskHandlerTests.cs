using Microsoft.Extensions.Time.Testing;
using TaskList.Api.Features.Tasks.ToggleTask;
using TaskList.Domain.Common;

namespace TaskList.IntegrationTests.Handlers;

public sealed class ToggleTaskHandlerTests
{
    private static readonly DateTimeOffset _now = new(2026, 5, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task When_TogglingExistingTask_Should_FlipCompletionAndPersist()
    {
        // Arrange
        await using var db = new TestDb();
        var task = TaskFaker.ATask(new FakeTimeProvider(_now));
        db.Context.Tasks.Add(task);
        await db.Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.Context.ChangeTracker.Clear();

        var handler = new ToggleTaskHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new ToggleTaskCommand(task.Id), TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsCompleted.ShouldBeTrue();

        db.Context.ChangeTracker.Clear();
        var persisted = await db.Context.Tasks
            .FirstOrDefaultAsync(t => t.Id == task.Id, TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
        persisted!.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public async Task When_TogglingMissingTask_Should_ReturnNotFoundFailure()
    {
        // Arrange
        await using var db = new TestDb();
        var handler = new ToggleTaskHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new ToggleTaskCommand(TaskId.New()), TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }
}
