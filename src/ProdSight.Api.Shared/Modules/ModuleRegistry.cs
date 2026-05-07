using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProdSight.Api.Shared.Modules;

public class ModuleRegistry
{
    private readonly Dictionary<IModule, bool> _moduleStates = new();
    private readonly ILogger<ModuleRegistry> _logger;
    
    public ModuleRegistry(ILogger<ModuleRegistry> logger, IEnumerable<IModule> modules)
    {
        _logger = logger;
        
        foreach (var module in modules)
        {
            _moduleStates[module] = false;
        }
        
        _logger.LogInformation("Discovered {Count} modules: {Modules}", _moduleStates.Count, string.Join(", ", _moduleStates.Keys.Select(m => m.Name)));
    }
    
    public void LoadModules(IServiceCollection services, IConfiguration config)
    {
        var log = new StringBuilder();
        log.AppendLine("Modules:");
        foreach (var module in _moduleStates.Keys)
        {
            var enabled = config.GetValue($"Modules:{module.Name}:Enabled", false);
            log.AppendLine($"\t- {module.Name}: {(enabled?"loaded":"disabled")}");
            if (enabled)
            {
                _moduleStates[module] = true;
            }
        }
        _logger.LogInformation(log.ToString().Trim());
        foreach (var module in _moduleStates.Where(p=>p.Value==true))
        {
            module.Key.RegisterServices(services, config);
        }
    }
    
    public void ConfigureModules(IEndpointRouteBuilder endpoints, IConfiguration config)
    {
        foreach (var kvp in _moduleStates.Where(kvp => kvp.Value))
        {
            kvp.Key.ConfigureEndpoints(endpoints);
        }
    }
    
    public IEnumerable<ModuleDescription> GetModulesStatus()
    {
        return _moduleStates.Select(kvp => new  ModuleDescription(kvp.Key.Name, kvp.Key.RequiredScope,kvp.Value));
    }
}