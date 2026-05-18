namespace TaskList.Domain.Common;

public sealed record DomainError(string Code, string Description, ErrorType Type)
{
    public static readonly DomainError None = new(string.Empty, string.Empty, ErrorType.None);

    public static DomainError NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);

    public static DomainError Conflict(string code, string description)
        => new(code, description, ErrorType.Conflict);
}
