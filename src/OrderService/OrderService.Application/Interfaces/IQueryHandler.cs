namespace OrderService.Application.Interfaces;

public interface IQueryHandler<in TQuery, TResult> where TResult : class?
{
    Task<TResult?> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
