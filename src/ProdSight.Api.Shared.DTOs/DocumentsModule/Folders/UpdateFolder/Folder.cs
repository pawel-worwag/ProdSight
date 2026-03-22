using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;

public class Folder
{
    [JsonPropertyName("id")] public required Guid Id { get; init; }
    [JsonPropertyName("parent-id")] public Guid? ParentId { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("created-at")] public required DateTimeOffset CreatedAt { get; init; }
}