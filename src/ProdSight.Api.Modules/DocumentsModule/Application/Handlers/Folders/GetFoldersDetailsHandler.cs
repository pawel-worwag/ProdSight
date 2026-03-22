using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class GetFoldersDetailsHandler(IFoldersRepository foldersRepository)
{
    public async Task<FolderDetails> HandleAsync(Guid folderId, CancellationToken ct = default)
    {
        var folder = await foldersRepository.GetAsync(folderId, ct);
        return new FolderDetails(folder.Id, folder.Name, folder.Description, folder.CreatedAt, folder.ParentId);
    }
}