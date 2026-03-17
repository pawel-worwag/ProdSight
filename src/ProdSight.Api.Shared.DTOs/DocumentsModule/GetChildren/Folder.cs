using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren;

public sealed record Folder(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt
);
