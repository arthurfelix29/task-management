using Shouldly;
using TaskList.Domain.Common;

namespace TaskList.UnitTests.Domain;

public sealed class ResultTests
{
    [Fact]
    public void Success_CarriesValue()
    {
        Result<int> result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
        result.Error.ShouldBe(DomainError.None);
    }

    [Fact]
    public void Failure_ExposesError()
    {
        var error = DomainError.NotFound("task.not_found", "Task was not found.");

        Result<int> result = Result.Failure<int>(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        var result = Result.Failure<int>(DomainError.NotFound("x", "y"));

        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess()
    {
        Result<string> result = "hello";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailure()
    {
        Result<string> result = DomainError.Conflict("conflict.code", "Conflict description.");

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Conflict);
    }

    [Fact]
    public void ImplicitConversion_FromError_ToNonGenericResult_ReturnsFailure()
    {
        Result result = DomainError.NotFound("task.not_found", "Task was not found.");

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Code.ShouldBe("task.not_found");
    }
}
