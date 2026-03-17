using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

public class FoldersRepository (DocumentsDbContext dbc)
    : IFoldersRepository
{
    public async Task<IReadOnlyList<Folder>> GetRootAsync(CancellationToken ct = default)
    {
        return await dbc.Folders
            .Where(f => f.ParentId == null)
            .OrderBy(f => f.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Folder>> GetChildrenAsync(Guid parentId, CancellationToken ct = default)
    {
        return await dbc.Folders
            .Where(f => f.ParentId == parentId)
            .OrderBy(f => f.Name)
            .ToListAsync(ct);
    }
}