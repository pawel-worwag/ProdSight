using System.Net.Http.Json;
using System.Text.Json;
using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;

namespace ProdSight.Frontend.Api;

public class ApiBroker(IHttpClientFactory httpFactory) : IApiBroker
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    
    private HttpClient GetClient(bool requireAuth) =>
        httpFactory.CreateClient(requireAuth ? "ApiAuthorized" : "Api");
    
    public async Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        var url = $"health";
        var res = await GetClient(false).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Empty or invalid health response.");
    }

    public async Task<User> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var url = $"identity/users";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<User>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Empty or invalid health response.");
    }
}