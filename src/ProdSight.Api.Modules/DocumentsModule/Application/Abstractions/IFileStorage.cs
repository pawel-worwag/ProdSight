namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

public interface IFileStorage
{
    Task<string> CreateAsync(Stream content, CancellationToken ct = default);

    Task<Stream> OpenReadAsync(string path, CancellationToken ct = default);

    Task DeleteAsync(string path, CancellationToken ct = default);
}
