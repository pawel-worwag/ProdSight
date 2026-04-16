using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

public sealed class DiskTemporaryFileStorage(IOptions<DiskTemporaryFileStorageOptions> options) : ITemporaryFileStorage
{
    public async Task<Guid> UploadAsync(Stream content, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        
        if (!content.CanRead)
            throw new ArgumentException("Content stream must be readable.", nameof(content));

        if (string.IsNullOrWhiteSpace(options.Value.RootDirectory))
            throw new ArgumentException("RootDirectory is required.", nameof(options.Value.RootDirectory));

        var rootDirectory = options.Value.RootDirectory;
        if (!Directory.Exists(rootDirectory))
            throw new DirectoryNotFoundException($"Temporary root directory '{rootDirectory}' not found.");

        var uploadId = Guid.CreateVersion7();
        var targetPath = Path.Combine(rootDirectory, uploadId.ToString("N"));

        await using var destination = File.Create(targetPath);
        await content.CopyToAsync(destination, ct);

        return uploadId;
    }
}
