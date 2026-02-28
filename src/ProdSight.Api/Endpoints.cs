using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Services;
using ProdSight.Api.Shared.Modules;
using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Api.Shared.DTOs.Status;
using HealthCheckResult = ProdSight.Api.Shared.DTOs.Health.HealthCheckResult;

namespace ProdSight.Api;

public static class Endpoints
{
    public static void MapStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/status", (IServiceProvider services) =>
        {
            var registry = services.GetRequiredService<ModuleRegistry>();
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
            var modules = registry.GetModulesStatus().Select(m => new ModuleStatus(m.Name, m.RequiredScope, m.Loaded));
            return new StatusResponse(version, modules);
        });
    }

    public static void MapHealthEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", (IServiceProvider sp) =>
        {
            var cache = sp.GetRequiredService<HealthCheckCacheService>();
            var results = cache.GetCachedResults();
            var overallStatus = results.All(r => r.Value.Status == HealthStatus.Healthy) ? "Healthy" : "Unhealthy";
            var lastChecked = cache.GetLastChecked();
            
            var checks = new Dictionary<string, IDictionary<string, HealthCheckResult>>();
            foreach (var (key, value) in results)
            {
                var cResult = new HealthCheckResult(
                    value.Status.ToString(),
                    value.Description,
                    value.Duration,
                    value.Tags,
                    value.Exception?.Message,
                    value.Data?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                );
                foreach (var tag in value.Tags)
                {
                    if (!checks.ContainsKey(tag))
                    {
                        checks.Add(tag,new Dictionary<string, HealthCheckResult>());
                    }
                    checks[tag].Add(key,cResult);
                }
            }
            return Results.Json(new HealthResponse(overallStatus, lastChecked, checks));
        });
    }
}