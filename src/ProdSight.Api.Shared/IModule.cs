using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ProdSight.Api.Shared;

public interface IModule
{
    string Name { get; }
    void RegisterServices(IServiceCollection services);
    void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
}