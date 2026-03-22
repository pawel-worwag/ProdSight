using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class CreateFolderHandler(IFoldersRepository foldersRepository)
{
    public async Task<Folder> HandleAsync(CreateFolderRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new BadRequestException("Name is required");
        var domainFolder = Domain.Entities.Folder.Create(req.Name.Trim(), req.Description, req.ParentId);
        var created = await foldersRepository.CreateAsync(domainFolder, ct);

        return new Folder
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        };
    }
}
