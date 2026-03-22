using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners;

public sealed record BusinessPartner
{
    [JsonPropertyName("id")] public required Guid Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("address")] public string? Address { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("created-at")] public required DateTimeOffset CreatedAt { get; init; }
}