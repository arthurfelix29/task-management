using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TaskList.Domain.Tasks;

namespace TaskList.UnitTests.Domain;

public sealed class TaskItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 14, 12, 0, 0, TimeSpan.Zero);
    private readonly FakeTimeProvider _clock = new(Now);

    [Fact]
    public void When_CreatingWithValidTitle_Should_InitializeWithDefaults()
    {
        var task = TaskItem.Create("Buy milk", _clock);

        task.Title.ShouldBe("Buy milk");
        task.IsCompleted.ShouldBeFalse();
        task.CreatedAt.ShouldBe(Now);
        task.Id.Value.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("", "empty string is rejected")]
    [InlineData("   ", "whitespace-only is rejected")]
    [InlineData(null, "null is rejected")]
    public void When_CreatingWithInvalidTitle_Should_RejectConstruction(string? title, string scenario)
    {
        _ = scenario;

        Should.Throw<ArgumentException>(() => TaskItem.Create(title!, _clock));
    }

    [Fact]
    public void When_CreatingWithNullClock_Should_RejectConstruction()
    {
        Should.Throw<ArgumentNullException>(() => TaskItem.Create("Buy milk", null!));
    }

    [Fact]
    public void When_TogglingPendingTask_Should_MarkAsCompleted()
    {
        var task = TaskItem.Create("Write tests", _clock);

        task.Toggle();

        task.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void When_TogglingCompletedTask_Should_RevertToPending()
    {
        var task = TaskItem.Create("Write tests", _clock);
        task.Toggle();

        task.Toggle();

        task.IsCompleted.ShouldBeFalse();
    }
}
