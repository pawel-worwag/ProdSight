using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

public interface IFoldersRepository
{
    Task<IReadOnlyList<Folder>> GetRootAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Folder>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
}