using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.Status;

public record StatusResponse(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("modules")] IEnumerable<ModuleStatus> Modules
);