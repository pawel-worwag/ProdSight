using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

public class FoldersRepository(DocumentsDbContext dbc)
    : IFoldersRepository
{
    public async Task<IReadOnlyList<Folder>> GetRootsAsync(CancellationToken ct = default)
    {
        return await dbc.Folders
            .AsNoTracking()
            .Where(f => f.ParentId == null)
            .OrderBy(f => f.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Folder>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbc.Folders
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Folder> GetAsync(Guid id, CancellationToken ct = default)
    {
        var folder = await dbc.Folders
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return  folder ?? throw new NotFoundException("Folder not found");
    }

    public async Task<IReadOnlyList<Folder>> GetChildrenAsync(Guid parentId, CancellationToken ct = default)
    {
        var parent = await dbc.Folders
            .AsNoTracking()
            .Include(p => p.Children)
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

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await dbc.SaveChangesAsync(ct);
    }
}