using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;

namespace ProdSight.Frontend.Api;

public interface IApiBroker
{
    Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default);
    Task<User> GetAllUsersAsync(CancellationToken cancellationToken = default);
}