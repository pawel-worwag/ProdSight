using ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak
{
    public interface IKeycloakApiBroker
    {
        Task<string> GetClientTokenAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<UserRepresentation>> GetUsersAsync(CancellationToken cancellationToken = default);
    }
}
