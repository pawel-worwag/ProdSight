using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails;
using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;


namespace ProdSight.Frontend.Api;

public interface IApiBroker
{
    Task<HealthResponse> GetHealthStatusAsync(CancellationToken cancellationToken = default);
    
    Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders.Folder>> GetRootFoldersAsync(
        CancellationToken cancellationToken = default);

    Task<FolderDetails> GetFolderDetailsAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetChildren.Folder>> GetChildrenFoldersAsync(
        Guid parentId, CancellationToken cancellationToken = default);

    Task<Folder> CreateFolderAsync(CreateFolderRequest request, CancellationToken cancellationToken = default);

    Task<ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner>>
        GetAllBusinessPartnersAsync(CancellationToken cancellationToken = default);
}