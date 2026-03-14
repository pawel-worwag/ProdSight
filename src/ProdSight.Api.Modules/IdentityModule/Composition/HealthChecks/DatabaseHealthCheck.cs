using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

namespace ProdSight.Api.Modules.IdentityModule.Composition.HealthChecks;

public class DatabaseHealthCheck(IdentityDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));
        
        try
        {
            var conn = db.Database.GetDbConnection();

            await conn.OpenAsync(cts.Token).ConfigureAwait(false);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            cmd.CommandTimeout = 5;
            await cmd.ExecuteScalarAsync(cts.Token).ConfigureAwait(false);
            await conn.CloseAsync();

            return HealthCheckResult.Healthy("Identity database is reachable and responds to queries");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Identity database is unreachable", ex);
        }
    }
}