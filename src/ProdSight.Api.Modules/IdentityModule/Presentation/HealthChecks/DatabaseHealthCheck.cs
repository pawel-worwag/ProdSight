using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

namespace ProdSight.Api.Modules.IdentityModule.Presentation.HealthChecks;

public class DatabaseHealthCheck(IdentityDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var conn = db.Database.GetDbConnection();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(15));

            try
            {
                await conn.OpenAsync(cts.Token).ConfigureAwait(false);

                using var cmd = conn.CreateCommand();
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
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Identity database is unreachable", ex);
        }
    }
}
