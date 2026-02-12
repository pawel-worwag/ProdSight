using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.Health;

public record HealthCheckResultDto(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("duration")] TimeSpan Duration,
    [property: JsonPropertyName("exception")] string? Exception,
    [property: JsonPropertyName("data")] Dictionary<string, object>? Data
);