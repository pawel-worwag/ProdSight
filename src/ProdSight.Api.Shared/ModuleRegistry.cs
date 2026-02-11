using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ProdSight.Api.Shared;

public class ModuleRegistry
{
    private readonly Dictionary<IModule, bool> _moduleStates = new();
    private readonly ILogger<ModuleRegistry> _logger;
    
    public ModuleRegistry(ILogger<ModuleRegistry> logger, IEnumerable<IModule> modules)
    {
        _logger = logger;
        
        foreach (var module in modules)
        {
            _moduleStates[module] = false;  // Domyślnie niezaładowany
        }
        
        _logger.LogInformation("Discovered {Count} modules: {Modules}", _moduleStates.Count, string.Join(", ", _moduleStates.Keys.Select(m => m.Name)));
    }
    
    public void LoadModules(IServiceCollection services, IConfiguration config)
    {
        foreach (var kvp in _moduleStates.ToList())  // Kopia do modyfikacji
        {
            var module = kvp.Key;
            var enabled = config.GetValue<bool>($"Modules:{module.Name}:Enabled", false);
            if (enabled)
            {
                _logger.LogInformation("Loading module: {ModuleName}", module.Name);
                module.RegisterServices(services);
                _moduleStates[module] = true;  // Oznacz jako załadowany
            }
        }
    }
    
    public void ConfigureModules(IEndpointRouteBuilder endpoints, IConfiguration config)
    {
        foreach (var kvp in _moduleStates.Where(kvp => kvp.Value))  // Tylko załadowane
        {
            kvp.Key.ConfigureEndpoints(endpoints);
        }
    }
}