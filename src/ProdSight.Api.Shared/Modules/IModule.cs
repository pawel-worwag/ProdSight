using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProdSight.Api.Shared.Modules;

public interface IModule
{
    string Name { get; }
    string RequiredScope { get; }  // Scope required from identity provider (e.g., Keycloak)
    void RegisterServices(IServiceCollection services,IConfiguration conf);
    void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
}