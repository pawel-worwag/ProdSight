using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;

namespace ProdSight.Api.Modules.IdentityModule.Presentation.Keycloak;

public class KeycloakHealthCheck (IKeycloakApiBroker apiBroker)
    : IHealthCheck
{


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var ok = await apiBroker.IsHealthyAsync(cancellationToken).ConfigureAwait(false);
            return ok ? HealthCheckResult.Healthy("Keycloak discovery endpoint OK") : HealthCheckResult.Unhealthy("Keycloak discovery endpoint failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}
