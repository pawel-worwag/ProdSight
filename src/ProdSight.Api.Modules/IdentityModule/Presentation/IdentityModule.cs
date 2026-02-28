using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Shared.Modules;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Extensions;
using ProdSight.Api.Modules.IdentityModule.Presentation.HelthChecks;
using ProdSight.Api.Shared.DTOs.Errors;

namespace ProdSight.Api.Modules.IdentityModule.Presentation;

public class IdentityModule : IModule
{
    public string Name => "IdentityModule";
    public string RequiredScope => "identity-module";

    public void RegisterServices(IServiceCollection services)
    {
        // Register Keycloak broker (preserve configuration from appsettings)
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.AddKeycloakBroker(config);
        services.AddIdentityDatabase(config);

        // Health checks: keep a dummy check and add Keycloak discovery check
        services.AddHealthChecks()
            .AddCheck<KeycloakHealthCheck>("identity-module-keycloak-integration-check", tags: ["identity-module"])
            .AddCheck<DatabaseHealthCheck>("identity-module-database-check", tags: ["identity-module"]);
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure identity endpoints here
        endpoints.MapGet("/identity", () => "Identity module endpoint")
            .WithTags("Identity Module");

        // Endpoint to get users from Keycloak
        endpoints.MapGet("/identity/users", async (IKeycloakApiBroker broker, CancellationToken ct) =>
        {
            var reps = await broker.GetUsersAsync(ct).ConfigureAwait(false);
            var list = reps.Select(r => new User
            {
                Id = r.Id ?? string.Empty,
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                EmailVerified = r.EmailVerified,
                Enabled = r.Enabled ?? false,
                Totp = r.Totp ?? false,
                CreatedTimestamp = r.CreatedTimestamp.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(r.CreatedTimestamp.Value).UtcDateTime
                    : DateTime.UnixEpoch
            }).ToList();

            return Results.Ok(list);
        })
        .WithTags("Identity Module")
        .Produces<List<User>>(StatusCodes.Status200OK,"application/json")
        .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }
}