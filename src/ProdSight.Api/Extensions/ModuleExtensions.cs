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
            var baseDir = AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            var modulesDir = Path.Combine(baseDir, "modules");
            if (Directory.Exists(modulesDir))
            {
                var dlls = Directory.GetFiles(modulesDir, "*.dll", SearchOption.AllDirectories);
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
        registry.ConfigureModules(endpoints, config);
        return endpoints;
    }
}