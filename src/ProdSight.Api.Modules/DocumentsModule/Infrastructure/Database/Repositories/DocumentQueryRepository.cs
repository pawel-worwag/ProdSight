using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Application.Models;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDocumentQueryRepository"/> for read-only document projections.
/// </summary>
public class DocumentQueryRepository(DocumentsDbContext dbc) : IDocumentQueryRepository
{
    public async Task<IReadOnlyList<DocumentListItem>> GetByFolderAsync(Guid folderId, CancellationToken ct = default)
    {
        return await dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.FolderId == folderId)
            .OrderBy(x => x.FileName)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new DocumentListItem(
                x.Id,
                x.FolderId,
                x.PartnerId,
                x.FileName,
                x.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DocumentListItem>> GetByBusinessPartnerAsync(Guid businessPartnerId, CancellationToken ct = default)
    {
        return await dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.PartnerId == businessPartnerId)
            .OrderBy(x => x.FileName)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new DocumentListItem(
                x.Id,
                x.FolderId,
                x.PartnerId,
                x.FileName,
                x.CreatedAt))
            .ToListAsync(ct);
    }
}
