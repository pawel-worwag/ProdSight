using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.CreateBusinessPartner;

public sealed record CreateBusinessPartnerRequest
{
    [JsonPropertyName("name")] public required string Name { get; init; } 
    [JsonPropertyName("address")] public string? Address { get; init; } 
    [JsonPropertyName("description")] public string? Description { get; init; } 
}