using System.Net.Http.Json;
using System.Text.Json;
using ProdSight.Api.Shared.DTOs.Health;

namespace ProdSight.Frontend.Api;

public class ApiBroker(HttpClient httpClient) : IApiBroker
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    public async Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        var url = $"health";
        var res = await httpClient.GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Empty or invalid health response.");
    }
}