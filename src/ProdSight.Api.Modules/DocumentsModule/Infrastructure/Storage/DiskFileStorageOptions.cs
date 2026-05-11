namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

public record DiskFileStorageOptions
{
    public required string RootDirectory { get; init; } = string.Empty;
}
