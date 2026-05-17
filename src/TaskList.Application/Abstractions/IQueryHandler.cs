namespace TaskList.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult> where TQuery : notnull
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
