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
            // Prefer AssemblyInformationalVersion (set by MinVer/CI), fallback to FileVersion or AssemblyVersion
            string version;
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var info = asm.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                if (!string.IsNullOrWhiteSpace(info))
                {
                    version = info;
                }
                else
                {
                    var fileVer = asm.GetCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>()?.Version;
                    version = fileVer ?? asm.GetName().Version?.ToString() ?? "Unknown";
                }
            }
            catch
            {
                version = "Unknown";
            }
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
            var checks = results.ToDictionary(
                r => r.Key,
                r => new HealthCheckResult(
                    r.Value.Status.ToString(),
                    r.Value.Description,
                    r.Value.Duration,
                    r.Value.Exception?.Message,
                    r.Value.Data?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                )
            );
            return Results.Json(new HealthResponse(overallStatus, lastChecked, checks));
        });
    }
}