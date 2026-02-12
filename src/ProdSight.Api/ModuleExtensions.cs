using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration config)
    {
        // Register modules explicitly
        services.AddSingleton<IModule, Modules.IdentityModule.Presentation.IdentityModule>();
        services.AddSingleton<IModule, Modules.MeasurementModule.Presentation.MeasurementModule>();
        
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ModuleRegistry>>();
        var registry = new ModuleRegistry(logger, services.BuildServiceProvider().GetServices<IModule>());
        registry.LoadModules(services, config);
        services.AddSingleton(registry);
        return services;
    }
    
    public static IEndpointRouteBuilder UseModules(this IEndpointRouteBuilder endpoints, IConfiguration config)
    {
        var registry = endpoints.ServiceProvider.GetRequiredService<ModuleRegistry>();  // DI
        registry.ConfigureModules(endpoints, config);
        return endpoints;
    }
}