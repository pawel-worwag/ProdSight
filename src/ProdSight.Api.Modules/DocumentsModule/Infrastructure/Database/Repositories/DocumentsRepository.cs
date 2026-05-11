using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDocumentsRepository"/> for aggregate-oriented persistence.
/// </summary>
public class DocumentsRepository(DocumentsDbContext dbc) : IDocumentsRepository
{
    public async Task<Document> CreateAsync(Document document, CancellationToken ct = default)
    {
        var entry = await dbc.Set<Document>().AddAsync(document, ct);
        await dbc.SaveChangesAsync(ct);
        return entry.Entity;
    }

    public async Task<Document?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await dbc.Set<Document>()
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await dbc.SaveChangesAsync(ct);
    }
}
