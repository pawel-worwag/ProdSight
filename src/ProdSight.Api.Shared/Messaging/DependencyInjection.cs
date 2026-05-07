using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProdSight.Api.Shared.Messaging;

public static class DependencyInjection
{
    private class HandlerRegistration;
    public static IServiceCollection AddRequestHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var log = new StringBuilder();
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<HandlerRegistration>>();
        log.AppendLine($"Registering handlers from assembly {assembly.FullName?.Split(',')[0]}:");
        var handlerInterfaceType = typeof(IRequestHandler<,>);
        var registrations = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new
            {
                ImplementationType = type.AsType(),
                ServiceTypes = type.ImplementedInterfaces
                    .Where(@interface => @interface.IsGenericType &&
                                         @interface.GetGenericTypeDefinition() == handlerInterfaceType)
                    .ToArray()
            })
            .Where(registration => registration.ServiceTypes.Length > 0);
        
        foreach (var registration in registrations)
        {
            foreach (var serviceType in registration.ServiceTypes)
            {
                services.AddScoped(serviceType, registration.ImplementationType);
                log.AppendLine($"\t- {serviceType.FullName?.Split(',')[0]?.Split('[')[2]}");
            }
        }

        logger.LogInformation(log.ToString().Trim());
        return services;
    }
}