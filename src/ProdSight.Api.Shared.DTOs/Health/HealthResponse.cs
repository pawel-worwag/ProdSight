using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.Health;

public record HealthResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("last-checked")] DateTime LastChecked,
    [property: JsonPropertyName("checks")]  Dictionary<string, IDictionary<string, HealthCheckResult>> Checks
);