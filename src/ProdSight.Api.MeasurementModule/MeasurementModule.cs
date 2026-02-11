using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Shared;

namespace ProdSight.Api.MeasurementModule;

public class MeasurementModule : IModule
{
    public string Name => "MeasurementModule";
    public string RequiredScope => "measurement-module";

    public void RegisterServices(IServiceCollection services)
    {
        // Register measurement services here
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Configure measurement endpoints here
        endpoints.MapGet("/measurement", () => "Measurement module endpoint");
    }
}