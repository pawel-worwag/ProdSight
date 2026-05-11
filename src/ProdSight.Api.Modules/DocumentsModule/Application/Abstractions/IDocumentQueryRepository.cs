using ProdSight.Api.Modules.DocumentsModule.Application.Models;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

/// <summary>
/// Read-only repository for lightweight document queries used by listings and reporting screens.
/// Returns projected models instead of domain aggregates.
/// </summary>
public interface IDocumentQueryRepository
{
    /// <summary>
    /// Returns lightweight document data for all documents assigned to the given folder.
    /// </summary>
    Task<IReadOnlyList<DocumentListItem>> GetByFolderAsync(
        Guid folderId,
        DocumentQuerySortBy sortBy = DocumentQuerySortBy.FileName,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default);

    /// <summary>
    /// Returns lightweight document data for all documents assigned to the given business partner.
    /// </summary>
    Task<IReadOnlyList<DocumentListItem>> GetByBusinessPartnerAsync(
        Guid businessPartnerId,
        DocumentQuerySortBy sortBy = DocumentQuerySortBy.FileName,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default);
}
