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
                    catch
                    {
                        // ignore individual assembly load errors
                    }
                }
            }
        }
        catch
        {
            // ignore module loading errors; registration below will fail fast if types missing
        }

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