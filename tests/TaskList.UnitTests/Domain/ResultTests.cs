using Shouldly;
using TaskList.Domain.Common;

namespace TaskList.UnitTests.Domain;

public sealed class ResultTests
{
    [Fact]
    public void When_CreatingSuccessResult_Should_CarryValueAndNoError()
    {
        Result<int> result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
        result.Error.ShouldBe(DomainError.None);
    }

    [Fact]
    public void When_CreatingFailureResult_Should_ExposeProvidedError()
    {
        var error = DomainError.NotFound("task.not_found", "Task was not found.");

        Result<int> result = Result.Failure<int>(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void When_AccessingValueOnFailure_Should_ThrowInvalidOperation()
    {
        var result = Result.Failure<int>(DomainError.NotFound("x", "y"));

        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void When_AssigningValueToGenericResult_Should_ProduceSuccess()
    {
        Result<string> result = "hello";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void When_AssigningErrorToGenericResult_Should_ProduceFailure()
    {
        Result<string> result = DomainError.Conflict("conflict.code", "Conflict description.");

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Conflict);
    }

    [Fact]
    public void When_AssigningErrorToNonGenericResult_Should_ProduceFailure()
    {
        Result result = DomainError.NotFound("task.not_found", "Task was not found.");

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Code.ShouldBe("task.not_found");
    }
}
