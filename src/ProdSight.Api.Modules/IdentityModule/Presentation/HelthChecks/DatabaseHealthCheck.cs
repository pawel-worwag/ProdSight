using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

namespace ProdSight.Api.Modules.IdentityModule.Presentation.HelthChecks;

public class DatabaseHealthCheck(IdentityDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);
            if (canConnect)
                return HealthCheckResult.Healthy("Identity database is reachable");

            return HealthCheckResult.Unhealthy("Identity database is not reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Identity database health check failed", ex);
        }
    }
}
