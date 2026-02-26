using System;
using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;

public record User
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("username")]
    public string? Username{ get; init; } = null;

    [JsonPropertyName("first-name")]
    public string? FirstName{ get; init; } = null;

    [JsonPropertyName("last-name")]
    public string? LastName{ get; init; } = null;

    [JsonPropertyName("email")]
    public string? Email{ get; init; } = null;

    [JsonPropertyName("email-verified")]
    public bool? EmailVerified{ get; init; } = null;

    [JsonPropertyName("enabled")]
    public bool Enabled{ get; init; } = false;

    [JsonPropertyName("created-timestamp")]
    public DateTime CreatedTimestamp { get; init; }

    [JsonPropertyName("totp")]
    public bool Totp{ get; init; } = false;
}