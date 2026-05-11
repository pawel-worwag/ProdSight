using System.Globalization;
using Microsoft.Extensions.Options;
using NUlid;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

public class DiskFileStorage(IOptions<DiskFileStorageOptions> options) : IFileStorage
{
    private readonly string _rootDirectory = GetRootDirectory(options.Value);

    public async Task<string> CreateAsync(Stream content, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (!content.CanRead)
            throw new ArgumentException("Content stream must be readable.", nameof(content));

        ct.ThrowIfCancellationRequested();

        var now = DateTimeOffset.UtcNow;
        var year = now.Year.ToString("0000", CultureInfo.InvariantCulture);
        var month = now.Month.ToString("00", CultureInfo.InvariantCulture);
        var fileName = Ulid.NewUlid().ToString();
        var relativePath = $"{year}/{month}/{fileName}";
        var directoryPath = Path.Combine(_rootDirectory, year, month);
        var fullPath = Path.Combine(directoryPath, fileName);

        Directory.CreateDirectory(directoryPath);

        await using var targetStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            options: FileOptions.Asynchronous);

        await content.CopyToAsync(targetStream, ct);

        return relativePath;
    }

    public Task<Stream> OpenReadAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var fullPath = GetExistingFilePath(path);
        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            options: FileOptions.Asynchronous);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var fullPath = GetExistingFilePath(path);

        File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string GetExistingFilePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is required.", nameof(path));

        var normalizedPath = NormalizeRelativePath(path);
        var fullPath = GetFullPath(normalizedPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File does not exist.", normalizedPath);

        return fullPath;
    }

    private static string NormalizeRelativePath(string path)
    {
        var normalizedPath = path.Replace('\\', '/').Trim('/');

        if (Path.IsPathRooted(path) || Path.IsPathRooted(normalizedPath))
            throw new InvalidOperationException("Absolute paths are not allowed.");

        var segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Any(segment => segment is "." or ".."))
            throw new InvalidOperationException("Path traversal was detected.");

        return normalizedPath;
    }

    private string GetFullPath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, relativePath));
        var rootWithSeparator = _rootDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? _rootDirectory
            : $"{_rootDirectory}{Path.DirectorySeparatorChar}";

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.Ordinal) &&
            !string.Equals(fullPath, _rootDirectory, StringComparison.Ordinal))
            throw new InvalidOperationException("The provided path points outside of the configured storage root.");

        return fullPath;
    }

    private static string GetRootDirectory(DiskFileStorageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.RootDirectory))
            throw new InvalidOperationException("DocumentsModule:DiskFileStorage:RootDirectory must be configured.");

        return Path.GetFullPath(options.RootDirectory);
    }
}
