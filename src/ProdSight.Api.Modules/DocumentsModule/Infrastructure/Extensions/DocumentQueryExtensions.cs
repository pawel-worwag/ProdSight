using ProdSight.Api.Modules.DocumentsModule.Application.Models;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;

public static class DocumentQueryExtensions
{
    public static IQueryable<Document> ApplySorting(
        this IQueryable<Document> query,
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
