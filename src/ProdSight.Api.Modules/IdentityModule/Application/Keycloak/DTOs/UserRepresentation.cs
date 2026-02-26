using System.Collections.Generic;

namespace ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs
{
    public record FederatedIdentityRepresentation(
        string? IdentityProvider,
        string? UserId,
        string? UserName
    );

    public record CredentialRepresentation(
        string? Id,
        string? Type,
        long? CreatedDate,
        string? UserLabel,
        bool? Temporary = null
    );

    public record UserConsentRepresentation(
        string? ClientId,
        IEnumerable<string>? GrantedClientScopes = null
    );

    public record SocialLinkRepresentation(
        string? ProviderId,
        string? SocialUserId,
        string? SocialUsername
    );

    public record UserRepresentation(
        string? Id = null,
        string? Username = null,
        string? FirstName = null,
        string? LastName = null,
        string? Email = null,
        bool? EmailVerified = null,
        IReadOnlyDictionary<string, IEnumerable<string>>? Attributes = null,
        object? UserProfileMetadata = null,
        bool? Enabled = null,
        string? Self = null,
        string? Origin = null,
        long? CreatedTimestamp = null,
        bool? Totp = null,
        string? FederationLink = null,
        string? ServiceAccountClientId = null,
        IEnumerable<CredentialRepresentation>? Credentials = null,
        IEnumerable<string>? DisableableCredentialTypes = null,
        IEnumerable<string>? RequiredActions = null,
        IEnumerable<FederatedIdentityRepresentation>? FederatedIdentities = null,
        IEnumerable<string>? RealmRoles = null,
        IReadOnlyDictionary<string, IEnumerable<string>>? ClientRoles = null,
        IEnumerable<UserConsentRepresentation>? ClientConsents = null,
        int? NotBefore = null,
        IReadOnlyDictionary<string, IEnumerable<string>>? ApplicationRoles = null,
        IEnumerable<SocialLinkRepresentation>? SocialLinks = null,
        IEnumerable<string>? Groups = null,
        IReadOnlyDictionary<string, bool>? Access = null
    );
}
