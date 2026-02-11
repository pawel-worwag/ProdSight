using ProdSight.Api;
using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks(); 
builder.Services.AddModules(builder.Configuration);
builder.Services.AddSingleton<HealthCheckCacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<HealthCheckCacheService>());



var app = builder.Build();


app.UseModules(builder.Configuration);

app.MapGet("/status", (IServiceProvider services) =>
{
    var registry = services.GetRequiredService<ProdSight.Api.Shared.ModuleRegistry>();
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    var modules = registry.GetModulesStatus();
    return new { Version = version, Modules = modules };
});

app.MapGet("/health", (IServiceProvider sp) => {
    var cache = sp.GetRequiredService<HealthCheckCacheService>();
    var results = cache.GetCachedResults();
    var keyValuePairs = results as KeyValuePair<string, HealthReportEntry>[] ?? results.ToArray();
    var overallStatus = keyValuePairs.All(r => r.Value.Status == HealthStatus.Healthy) ? "Healthy" : "Unhealthy";
    return Results.Json(new { Status = overallStatus, Checks = keyValuePairs.ToDictionary(r => r.Key, r => r.Value) });
});

app.Run();