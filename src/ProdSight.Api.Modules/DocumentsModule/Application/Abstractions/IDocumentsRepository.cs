using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

/// <summary>
/// Repository for working with the <see cref="Document"/> aggregate root.
/// Use it for create/load/save operations that should preserve aggregate boundaries and domain invariants.
/// </summary>
public interface IDocumentsRepository
{
    /// <summary>
    /// Persists a new <see cref="Document"/> aggregate.
    /// </summary>
    Task<Document> CreateAsync(Document document, CancellationToken ct = default);

    /// <summary>
    /// Loads a <see cref="Document"/> aggregate together with data required to continue domain operations.
    /// </summary>
    Task<Document?> GetAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Saves changes made to already loaded aggregates.
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
