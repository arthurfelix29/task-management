using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TaskList.Domain.Tasks;

namespace TaskList.UnitTests.Domain;

public sealed class TaskItemTests
{
    private static readonly DateTimeOffset _now = new(2026, 5, 14, 12, 0, 0, TimeSpan.Zero);
    private readonly FakeTimeProvider _clock = new(_now);

    [Fact]
    public void When_CreatingWithValidTitle_Should_InitializeWithDefaults()
    {
        // Act
        var task = TaskItem.Create("Buy milk", _clock);

        // Assert
        task.Title.ShouldBe("Buy milk");
        task.IsCompleted.ShouldBeFalse();
        task.CreatedAt.ShouldBe(_now);
        task.Id.Value.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("", "empty string is rejected")]
    [InlineData("   ", "whitespace-only is rejected")]
    [InlineData(null, "null is rejected")]
    public void When_CreatingWithInvalidTitle_Should_RejectConstruction(string? title, string scenario)
    {
        _ = scenario;

        // Act / Assert
        Should.Throw<ArgumentException>(() => TaskItem.Create(title!, _clock));
    }

    [Fact]
    public void When_CreatingWithNullClock_Should_RejectConstruction()
    {
        // Act / Assert
        Should.Throw<ArgumentNullException>(() => TaskItem.Create("Buy milk", null!));
    }

    [Fact]
    public void When_TogglingPendingTask_Should_MarkAsCompleted()
    {
        // Arrange
        var task = TaskItem.Create("Write tests", _clock);

        // Act
        task.Toggle();

        // Assert
        task.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void When_TogglingCompletedTask_Should_RevertToPending()
    {
        // Arrange
        var task = TaskItem.Create("Write tests", _clock);
        task.Toggle();

        // Act
        task.Toggle();

        // Assert
        task.IsCompleted.ShouldBeFalse();
    }
}
