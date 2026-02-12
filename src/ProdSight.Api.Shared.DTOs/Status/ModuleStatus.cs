using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.Status;

public record ModuleStatus(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("required-scope")] string RequiredScope,
    [property: JsonPropertyName("loaded")] bool Loaded
);