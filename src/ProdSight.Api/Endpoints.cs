using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Services;
using ProdSight.Api.Shared;

namespace ProdSight.Api;

public static class Endpoints
{
    public static void MapStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/status", (IServiceProvider services) =>
        {
            var registry = services.GetRequiredService<ModuleRegistry>();
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
            var modules = registry.GetModulesStatus();
            return new { Version = version, Modules = modules };
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
            return Results.Json(new { Status = overallStatus, LastChecked = lastChecked, Checks = results.ToDictionary(r => r.Key, r => r.Value) });
        });
    }
}