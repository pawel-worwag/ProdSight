using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProdSight.Api.Services;

public class HealthCheckCacheService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ConcurrentDictionary<string, HealthReportEntry> _cachedResults = new();
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);
    private DateTime _lastChecked = DateTime.MinValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateHealthChecksAsync(stoppingToken);
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
    
    private async Task UpdateHealthChecksAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var healthCheckService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();
        
        var report = await healthCheckService.CheckHealthAsync(cancellationToken);

        foreach (var entry in report.Entries)
        {
            _cachedResults[entry.Key] = entry.Value;
        }

        _lastChecked = DateTime.UtcNow;
    }

    public IEnumerable<KeyValuePair<string, HealthReportEntry>> GetCachedResults()
    {
        return _cachedResults.ToArray();
    }

    public DateTime GetLastChecked() => _lastChecked;
}