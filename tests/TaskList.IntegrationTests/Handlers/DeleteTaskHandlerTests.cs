using Microsoft.Extensions.Time.Testing;
using TaskList.Api.Features.Tasks.DeleteTask;
using TaskList.Domain.Common;

namespace TaskList.IntegrationTests.Handlers;

public sealed class DeleteTaskHandlerTests
{
    private static readonly DateTimeOffset _now = new(2026, 5, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task When_DeletingExistingTask_Should_RemoveFromDatabase()
    {
        // Arrange
        await using var db = new TestDb();
        var task = TaskFaker.ATask(new FakeTimeProvider(_now));
        db.Context.Tasks.Add(task);
        await db.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new DeleteTaskHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new DeleteTaskCommand(task.Id), TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var stillThere = await db.Context.Tasks
            .AnyAsync(t => t.Id == task.Id, TestContext.Current.CancellationToken);
        stillThere.ShouldBeFalse();
    }

    [Fact]
    public async Task When_DeletingMissingTask_Should_ReturnNotFoundFailure()
    {
        // Arrange
        await using var db = new TestDb();
        var handler = new DeleteTaskHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new DeleteTaskCommand(TaskId.New()), TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }
}
