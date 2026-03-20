using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

public interface IFoldersRepository
{
    Task<IReadOnlyList<Folder>> GetRootsAsync(CancellationToken ct = default);
    Task<Folder> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<IReadOnlyList<Folder>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<Folder> CreateAsync(Folder folder, CancellationToken ct = default);
}