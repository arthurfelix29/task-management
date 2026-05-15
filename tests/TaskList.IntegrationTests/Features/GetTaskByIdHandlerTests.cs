using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TaskList.Api.Features.Tasks.GetTaskById;
using TaskList.Domain.Common;
using TaskList.Domain.Tasks;
using TaskList.IntegrationTests.Fixtures;

namespace TaskList.IntegrationTests.Features;

public sealed class GetTaskByIdHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task When_GettingExistingTaskById_Should_ReturnMappedResponse()
    {
        // Arrange
        await using var db = new TestDb();
        var task = TaskItem.Create("Pay rent", new FakeTimeProvider(Now));
        db.Context.Tasks.Add(task);
        await db.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetTaskByIdHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new GetTaskByIdQuery(task.Id), TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(task.Id.Value);
        result.Value.Title.ShouldBe("Pay rent");
        result.Value.CreatedAt.ShouldBe(Now);
    }

    [Fact]
    public async Task When_GettingMissingTaskById_Should_ReturnNotFoundFailure()
    {
        // Arrange
        await using var db = new TestDb();
        var handler = new GetTaskByIdHandler(db.Context);

        // Act
        var result = await handler.HandleAsync(new GetTaskByIdQuery(TaskId.New()), TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Code.ShouldBe("task.not_found");
    }
}
