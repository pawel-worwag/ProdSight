using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFoldersTree;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public class GetFoldersTreeHandler(IFoldersRepository foldersRepository)
{
    public async Task<ICollection<Folder>> HandleAsync(
        CancellationToken ct = default)
    {
        var records = await foldersRepository.GetAllAsync(ct);

        var tree = records.ToDictionary(r => r.Id, r => new Folder()
        {
            Id = r.Id,
            ParentId = r.ParentId,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            Children = new List<Folder>()
        });

        foreach (var t in tree)
        {
            if (t.Value.ParentId != null)
            {
                var pId = t.Value.ParentId.Value;
                tree[pId].Children.Add(t.Value);
            }
        }
        
        return tree.Where(f=>f.Value.ParentId == null)
            .Select(f => f.Value).ToList();
    }
}