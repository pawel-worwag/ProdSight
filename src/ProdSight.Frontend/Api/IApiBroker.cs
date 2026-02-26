using ProdSight.Api.Shared.DTOs.Health;

namespace ProdSight.Frontend.Api;

public interface IApiBroker
{
    Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default);
}