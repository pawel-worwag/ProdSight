namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

internal static class DiskFileStoragePathValidator
{
    internal static string NormalizeRelativePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is required.", nameof(path));

        var normalizedPath = path.Replace('\\', '/').Trim('/');

        if (Path.IsPathRooted(path) || Path.IsPathRooted(normalizedPath))
            throw new InvalidOperationException("Absolute paths are not allowed.");

        var segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Any(segment => segment is "." or ".."))
            throw new InvalidOperationException("Path traversal was detected.");

        return normalizedPath;
    }
}
