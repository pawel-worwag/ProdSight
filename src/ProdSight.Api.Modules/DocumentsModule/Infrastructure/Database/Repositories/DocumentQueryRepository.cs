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
    public async Task<IReadOnlyList<DocumentListItem>> GetByFolderAsync(
        Guid folderId,
        DocumentQuerySortBy sortBy = DocumentQuerySortBy.FileName,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default)
    {
        return await ApplySorting(
                dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.FolderId == folderId),
                sortBy,
                sortDirection)
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
        return await ApplySorting(
                dbc.Set<Document>()
            .AsNoTracking()
            .Where(x => x.PartnerId == businessPartnerId),
                sortBy,
                sortDirection)
            .Select(x => new DocumentListItem(
                x.Id,
                x.FolderId,
                x.PartnerId,
                x.FileName,
                x.CreatedAt))
            .ToListAsync(ct);
    }

    private static IQueryable<Document> ApplySorting(
        IQueryable<Document> query,
        DocumentQuerySortBy sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (DocumentQuerySortBy.CreatedAt, SortDirection.Ascending) => query
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.CreatedAt, SortDirection.Descending) => query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.Id, SortDirection.Ascending) => query
                .OrderBy(x => x.Id)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.Id, SortDirection.Descending) => query
                .OrderByDescending(x => x.Id)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.FolderId, SortDirection.Ascending) => query
                .OrderBy(x => x.FolderId)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.FolderId, SortDirection.Descending) => query
                .OrderByDescending(x => x.FolderId)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.BusinessPartnerId, SortDirection.Ascending) => query
                .OrderBy(x => x.PartnerId)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.BusinessPartnerId, SortDirection.Descending) => query
                .OrderByDescending(x => x.PartnerId)
                .ThenBy(x => x.FileName),
            (DocumentQuerySortBy.FileName, SortDirection.Descending) => query
                .OrderByDescending(x => x.FileName)
                .ThenBy(x => x.CreatedAt),
            _ => query
                .OrderBy(x => x.FileName)
                .ThenBy(x => x.CreatedAt)
        };
    }
}
