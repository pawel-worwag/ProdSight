using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder;

public record Folder
{
    [JsonPropertyName("id")] public required Guid Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("created-at")] public required DateTimeOffset CreatedAt { get; init; }
};
