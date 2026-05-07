namespace ProdSight.Api.Shared.Messaging;

public interface IRequestHandler<in TQuery, TResult>
    where TQuery : IRequest<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}