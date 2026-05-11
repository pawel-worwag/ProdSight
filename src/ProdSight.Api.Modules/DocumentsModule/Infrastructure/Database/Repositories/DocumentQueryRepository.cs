using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Application.Models;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDocumentQueryRepository"/> for read-only document projections.
/// </summary>
public class DocumentQueryRepository(DocumentsDbContext dbc) : IDocumentQueryRepository
{
    public async Task<IReadOnlyList<DocumentListItem>> GetByFolderAsync(
        Guid folderId,
        DocumentQuerySortBy sortBy = DocumentQuerySortBy.FileName,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default)
    {
        return await dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.FolderId == folderId)
            .ApplySorting(sortBy, sortDirection)
            .Select(x => new DocumentListItem(
                x.Id,
                x.FolderId,
                x.PartnerId,
                x.FileName,
                x.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DocumentListItem>> GetByBusinessPartnerAsync(
        Guid businessPartnerId,
        DocumentQuerySortBy sortBy = DocumentQuerySortBy.FileName,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default)
    {
        return await dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.PartnerId == businessPartnerId)
            .ApplySorting(sortBy, sortDirection)
            .Select(x => new DocumentListItem(
                x.Id,
                x.FolderId,
                x.PartnerId,
                x.FileName,
                x.CreatedAt))
            .ToListAsync(ct);
    }
}
