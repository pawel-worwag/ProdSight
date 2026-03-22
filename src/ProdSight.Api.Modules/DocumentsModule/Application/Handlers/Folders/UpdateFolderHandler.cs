using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class UpdateFolderHandler(IFoldersRepository foldersRepository)
{
    public async Task<Folder> HandleAsync(Guid id, UpdateFolderRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new BadRequestException("Name is required");
        var folder = await foldersRepository.GetAsync(id, ct);
        
        folder.Rename(req.Name);
        folder.ChangeDescription(req.Description);
        folder.ChangeParent(req.ParentId);
        await foldersRepository.SaveChangesAsync(ct);

        return new Folder()
        {
            Id = folder.Id,
            ParentId = folder.ParentId,
            Name = folder.Name,
            CreatedAt = folder.CreatedAt,
            Description = folder.Description
        };
        
    }
}