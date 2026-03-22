namespace ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;

public class Folder
{
    public required Guid Id { get; init; }
    public Guid? ParentId { get; init; }
    public required string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}