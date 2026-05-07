using Microsoft.AspNetCore.Routing;

namespace ProdSight.Api.Shared.Api;

public interface IApiEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder endpoints);
}