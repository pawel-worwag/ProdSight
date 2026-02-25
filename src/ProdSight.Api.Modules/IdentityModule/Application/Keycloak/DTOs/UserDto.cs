namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs
{
    public record UserDto(
        string Id,
        string Username,
        string? Email,
        bool Enabled
    );
}
