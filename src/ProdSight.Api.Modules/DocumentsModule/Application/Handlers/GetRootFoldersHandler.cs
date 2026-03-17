using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers;

public class GetRootFoldersHandler(IFoldersRepository foldersRepository)
{
    public async Task<IReadOnlyList<Folder>> HandleAsync(CancellationToken ct = default)
    {
        var folders = await foldersRepository.GetRootAsync(ct);
        return folders.Select(f => new Folder(f.Id, f.Name, f.Description, f.CreatedAt)).ToList();
    }
}
