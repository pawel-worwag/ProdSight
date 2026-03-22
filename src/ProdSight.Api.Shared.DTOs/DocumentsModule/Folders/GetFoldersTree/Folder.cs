using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFoldersTree;

public sealed record Folder
{
    [JsonPropertyName("id")] public required Guid Id { get; init; }
    [JsonPropertyName("parent-id")] public Guid? ParentId { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("created-at")] public DateTimeOffset CreatedAt { get; init; }
    [JsonPropertyName("children")] public ICollection<Folder> Children { get; init; } = new List<Folder>();
}