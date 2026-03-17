using System;
using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders;

public record Folder(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("created-at")] DateTimeOffset CreatedAt
);
