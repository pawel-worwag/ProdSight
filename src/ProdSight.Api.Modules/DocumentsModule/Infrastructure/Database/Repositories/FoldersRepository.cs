using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

public class FoldersRepository(DocumentsDbContext dbc)
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
        var parent = await dbc.Folders.Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == parentId, ct);
        return parent is null
            ? throw new BadRequestException("Parent folder not found")
            : parent.Children.OrderBy(f => f.Name).ToList();
    }

    public async Task<Folder> CreateAsync(Folder folder, CancellationToken ct = default)
    {
        var entry = await dbc.Folders.AddAsync(folder, ct);
        await dbc.SaveChangesAsync(ct);
        return entry.Entity;
    }
}