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
        const string url = $"api/v1/health";
        var res = await GetClient(false).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        const string url = $"api/v1/identity/users";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<IList<User>>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder>> GetRootFoldersAsync(CancellationToken cancellationToken = default)
    {
        const string url = $"api/v1/documents/folders/root";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
         return await res.Content.ReadFromJsonAsync<IList<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder>>(JsonOptions, cancellationToken)
             ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ProdSight.Api.Shared.DTOs.DocumentsModule.GetFolderDetails.FolderDetails> GetFolderDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var url = $"api/v1/documents/folders/{id}/details";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
         return await res.Content.ReadFromJsonAsync<ProdSight.Api.Shared.DTOs.DocumentsModule.GetFolderDetails.FolderDetails>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren.Folder>> GetChildrenFoldersAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var url = $"api/v1/documents/folders/{parentId}/children";
        var res = await GetClient(true).GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
         return await res.Content.ReadFromJsonAsync<IList<ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren.Folder>>(JsonOptions, cancellationToken)
             ?? throw new InvalidOperationException("Empty or invalid response.");
    }

    public async Task<ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.Folder> CreateFolderAsync(ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.CreateFolderRequest request, CancellationToken cancellationToken = default)
    {
        const string url = $"api/v1/documents/folders";
        var res = await GetClient(true).PostAsJsonAsync(url, request, JsonOptions, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.Folder>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Empty or invalid response.");
    }
}