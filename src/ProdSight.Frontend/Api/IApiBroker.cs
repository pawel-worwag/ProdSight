using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;


namespace ProdSight.Frontend.Api;

public interface IApiBroker
{
    Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default);
    Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder>> GetRootFoldersAsync(CancellationToken cancellationToken = default);

    Task<ProdSight.Api.Shared.DTOs.DocumentsModule.GetFolderDetails.FolderDetails> GetFolderDetailsAsync(Guid id,
        CancellationToken cancellationToken = default);
    Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren.Folder>> GetChildrenFoldersAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.Folder> CreateFolderAsync(ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.CreateFolderRequest request, CancellationToken cancellationToken = default);
}