using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace ProdSight.Api.Shared.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints,Assembly assembly)
    {
        var endpointTypes = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                 typeof(IApiEndpoint).IsAssignableFrom(type))
            .Select(type => type.AsType())
            .OrderBy(type => type.FullName, StringComparer.Ordinal);
        foreach (var endpointType in endpointTypes)
        {
            if (Activator.CreateInstance(endpointType) is not IApiEndpoint endpoint)
            {
                throw new InvalidOperationException($"Could not create endpoint instance for '{endpointType.FullName}'.");
            }
            endpoint.MapEndpoint(endpoints);
        }
        return endpoints;
    }
}