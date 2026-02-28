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
        const string url = $"health";
        var res = await GetClient(false).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        const string url = $"identity/users";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<IList<User>>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Empty or invalid response.");
    }
}