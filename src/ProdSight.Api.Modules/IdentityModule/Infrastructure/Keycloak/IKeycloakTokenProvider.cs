namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak;

public interface IKeycloakTokenProvider
{
    Task<string> GetClientTokenAsync(CancellationToken cancellationToken = default);
}