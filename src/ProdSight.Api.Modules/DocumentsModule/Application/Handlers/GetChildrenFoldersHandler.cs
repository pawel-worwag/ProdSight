using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers;

public class GetChildrenFoldersHandler(IFoldersRepository foldersRepository)
{
    public async Task<IReadOnlyList<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder>> HandleAsync(Guid parentId, CancellationToken ct = default)
    {
        var folders = await foldersRepository.GetChildrenAsync(parentId, ct);
        return folders.Select(f => new ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder(f.Id, f.Name, f.Description, f.CreatedAt)).ToList();
    }
}
