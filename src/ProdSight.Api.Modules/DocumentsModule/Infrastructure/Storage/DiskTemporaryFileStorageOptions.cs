namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

public record DiskTemporaryFileStorageOptions
{
    public required string RootDirectory { get; init; } = string.Empty;
}