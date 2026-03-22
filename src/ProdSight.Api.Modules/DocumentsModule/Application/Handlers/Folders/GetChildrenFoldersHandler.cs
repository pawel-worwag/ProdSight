using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class GetChildrenFoldersHandler(IFoldersRepository foldersRepository)
{
    public async Task<IReadOnlyList<Folder>> HandleAsync(Guid parentId, CancellationToken ct = default)
    {
        var folders = await foldersRepository.GetChildrenAsync(parentId, ct);
        return folders.Select(f => new Folder
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            CreatedAt = f.CreatedAt
        }).ToList();
    }
}
