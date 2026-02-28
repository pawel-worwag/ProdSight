using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api.Modules.MeasurementModule.Presentation;

public class MeasurementModule : IModule
{
    public string Name => "MeasurementModule";
    public string RequiredScope => "measurement-module";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck("measurement-module-dummy-check", () => HealthCheckResult.Healthy("Measurement module is healthy"), tags: ["measurement-module"]);
        // Register measurement services here
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure measurement endpoints here
        endpoints.MapGet("/measurement", () => "Measurement module endpoint");
    }
}