using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TaskList.Domain.Tasks;

namespace TaskList.UnitTests.Domain;

public sealed class TaskItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 14, 12, 0, 0, TimeSpan.Zero);
    private readonly FakeTimeProvider _clock = new(Now);

    [Fact]
    public void Create_WithValidTitle_SetsExpectedDefaults()
    {
        var task = TaskItem.Create("Buy milk", _clock);

        task.Title.ShouldBe("Buy milk");
        task.IsCompleted.ShouldBeFalse();
        task.CreatedAt.ShouldBe(Now);
        task.Id.Value.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsArgumentException(string title)
    {
        Should.Throw<ArgumentException>(() => TaskItem.Create(title, _clock));
    }

    [Fact]
    public void Create_WithNullTitle_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => TaskItem.Create(null!, _clock));
    }

    [Fact]
    public void Create_WithNullClock_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => TaskItem.Create("Buy milk", null!));
    }

    [Fact]
    public void Toggle_FromIncomplete_MarksCompleted()
    {
        var task = TaskItem.Create("Write tests", _clock);

        task.Toggle();

        task.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void Toggle_FromCompleted_MarksIncomplete()
    {
        var task = TaskItem.Create("Write tests", _clock);
        task.Toggle();

        task.Toggle();

        task.IsCompleted.ShouldBeFalse();
    }
}
