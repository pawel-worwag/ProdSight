using System.Runtime.Loader;
using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api.Extensions;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration config)
    {
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ModuleRegistry>>();
        try
        {
            var modulesDir = AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(modulesDir))
            {
                var dlls = Directory.GetFiles(modulesDir, "ProdSight.Api.Modules.*.dll", SearchOption.AllDirectories);
                foreach (var dll in dlls)
                {
                    try
                    {
                        var name = Path.GetFileNameWithoutExtension(dll);
                        var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                            .Any(a => string.Equals(a.GetName().Name, name, StringComparison.OrdinalIgnoreCase));
                        if (!alreadyLoaded)
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Failed to load assembly from '{dll}': {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading modules");
        }
        
        
        
        var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
            })
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();
        
        
        
        foreach (var mt in moduleTypes)
        {
            try
            {
                services.AddSingleton(typeof(IModule), mt);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to register module type {mt.FullName}: {ex.Message}");
            }
        }
        
        //OLD---------

        var registry = new ModuleRegistry(logger, services.BuildServiceProvider().GetServices<IModule>());
        registry.LoadModules(services, config);
        services.AddSingleton(registry);
 
        return services;
    }
    
    public static IEndpointRouteBuilder UseModules(this IEndpointRouteBuilder endpoints, IConfiguration config)
    {
        var registry = endpoints.ServiceProvider.GetRequiredService<ModuleRegistry>();  // DI
        var api = endpoints.MapGroup("api");
        registry.ConfigureModules(api, config);
        return endpoints;
    }
}