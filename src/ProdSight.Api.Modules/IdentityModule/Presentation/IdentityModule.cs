using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api.Modules.IdentityModule.Presentation;

public class IdentityModule : IModule
{
    public string Name => "IdentityModule";
    public string RequiredScope => "identity-module";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck("identity-module-dummy-check", () => HealthCheckResult.Healthy("Identity module is healthy"));
        // Register Keycloak or identity services here
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure identity endpoints here
        endpoints.MapGet("/identity", () => "Identity module endpoint");
    }
}