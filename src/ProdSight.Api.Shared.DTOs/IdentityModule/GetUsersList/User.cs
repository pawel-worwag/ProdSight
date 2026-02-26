namespace ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;

public record User
{
    public required string Id { get; init; }
    public string? Username{ get; init; } = null;
    public string? FirstName{ get; init; } = null;
    public string? LastName{ get; init; } = null;
    public string? Email{ get; init; } = null;
    public bool? EmailVerified{ get; init; } = null;
    public bool Enabled{ get; init; } = false;
    public DateTime CreatedTimestamp { get; init; }
    public bool Totp{ get; init; } = false;
}