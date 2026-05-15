namespace TaskList.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, DomainError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public DomainError Error { get; }

    public static Result Success() => new(true, DomainError.None);

    public static Result<T> Success<T>(T value) => new(value);

    public static Result Failure(DomainError error) => new(false, error);

    public static Result<T> Failure<T>(DomainError error) => new(error);

    public static implicit operator Result(DomainError error) => Failure(error);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T value) : base(true, DomainError.None) => _value = value;

    internal Result(DomainError error) : base(false, error) => _value = default;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed Result.");

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(DomainError error) => new(error);
}
