using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;

namespace ProdSight.Api.Modules.IdentityModule.Presentation.HelthChecks;

public class KeycloakHealthCheck (IKeycloakApiBroker apiBroker)
    : IHealthCheck
{


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var ok = await apiBroker.IsHealthyAsync(cancellationToken).ConfigureAwait(false);
            return ok ? HealthCheckResult.Healthy("Keycloak discovery endpoint is reachable and responds") : HealthCheckResult.Unhealthy("Keycloak discovery endpoint is unreachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Keycloak discovery endpoint is unreachable", ex);
        }
    }
}
