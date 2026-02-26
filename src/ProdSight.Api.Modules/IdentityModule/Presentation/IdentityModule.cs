using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Shared.Modules;
using Microsoft.Extensions.Configuration;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Extensions;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;

namespace ProdSight.Api.Modules.IdentityModule.Presentation;

public class IdentityModule : IModule
{
    public string Name => "IdentityModule";
    public string RequiredScope => "identity-module";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck("identity-module-dummy-check", () => HealthCheckResult.Healthy("Identity module is healthy"));
        // Register Keycloak broker (preserve configuration from appsettings)
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.AddKeycloakBroker(config);
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure identity endpoints here
        endpoints.MapGet("/identity", () => "Identity module endpoint");

        // Endpoint to get users from Keycloak
        endpoints.MapGet("/identity/users", async (IKeycloakApiBroker broker, CancellationToken ct) =>
            await broker.GetUsersAsync(ct).ConfigureAwait(false)
        );
    }
}