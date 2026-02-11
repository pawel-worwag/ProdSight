using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Shared;

namespace ProdSight.Api.Modules.IdentityModule.Presentation;

public class IdentityModule : IModule
{
    public string Name => "IdentityModule";
    public string RequiredScope => "identity-module";

    public void RegisterServices(IServiceCollection services)
    {
        // Register Keycloak or identity services here
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure identity endpoints here
        endpoints.MapGet("/identity", () => "Identity module endpoint");
    }
}