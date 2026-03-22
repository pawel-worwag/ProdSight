using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class GetRootFoldersHandler(IFoldersRepository foldersRepository)
{
    public async Task<IReadOnlyList<Folder>> HandleAsync(CancellationToken ct = default)
    {
        var folders = await foldersRepository.GetRootsAsync(ct);
        return folders.Select(f => new Folder
        {
            Id = f.Id,
            Name = f.Name,
            CreatedAt = f.CreatedAt
        }).ToList();
    }
}
