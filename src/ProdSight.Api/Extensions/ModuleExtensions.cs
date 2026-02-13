using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api.Extensions;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration config)
    {
        // Load module assemblies from publish/modules if present so types referenced in code can be resolved
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
            Console.Error.WriteLine($"Module loader error: {ex.Message}");
        }

        // Register discovered modules by scanning loaded assemblies instead of
        // referencing module types directly (avoids FileNotFound when assemblies
        // are loaded dynamically into subfolders).
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