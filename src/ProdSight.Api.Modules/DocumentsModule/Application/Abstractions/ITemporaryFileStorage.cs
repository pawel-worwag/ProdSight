namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

public interface ITemporaryFileStorage
{
    Task<Guid> UploadAsync(Stream content, CancellationToken ct = default);
}