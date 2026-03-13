using System.Text.Json.Serialization;

namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs
{
    public sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
