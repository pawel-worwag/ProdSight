using ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak
{
    public interface IKeycloakApiBroker
    {
        Task<IEnumerable<UserRepresentation>> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
    }
}
