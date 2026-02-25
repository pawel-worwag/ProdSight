namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak
{
    public class KeycloakOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Realm { get; set; } = "master";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
