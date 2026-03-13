using System.Text.Json.Serialization;

namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

public record DiscoveryResponse(
    [property: JsonPropertyName("issuer")] string? Issuer,
    [property: JsonPropertyName("authorization_endpoint")] string? AuthorizationEndpoint,
    [property: JsonPropertyName("token_endpoint")] string? TokenEndpoint,
    [property: JsonPropertyName("userinfo_endpoint")] string? UserinfoEndpoint,
    [property: JsonPropertyName("end_session_endpoint")] string? EndSessionEndpoint,
    [property: JsonPropertyName("jwks_uri")] string? JwksUri,
    [property: JsonPropertyName("introspection_endpoint")] string? IntrospectionEndpoint,
    [property: JsonPropertyName("revocation_endpoint")] string? RevocationEndpoint,
    [property: JsonPropertyName("response_types_supported")] string[]? ResponseTypesSupported,
    [property: JsonPropertyName("grant_types_supported")] string[]? GrantTypesSupported,
    [property: JsonPropertyName("scopes_supported")] string[]? ScopesSupported,
    [property: JsonPropertyName("claims_supported")] string[]? ClaimsSupported,
    [property: JsonPropertyName("id_token_signing_alg_values_supported")] string[]? IdTokenSigningAlgValuesSupported,
    [property: JsonPropertyName("code_challenge_methods_supported")] string[]? CodeChallengeMethodsSupported,
    [property: JsonPropertyName("token_endpoint_auth_methods_supported")] string[]? TokenEndpointAuthMethodsSupported
);
