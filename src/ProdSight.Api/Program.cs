using ProdSight.Api;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(builder.Configuration);
var app = builder.Build();

app.UseModules(builder.Configuration);

app.MapGet("/status", (IServiceProvider services) =>
{
    var registry = services.GetRequiredService<ProdSight.Api.Shared.ModuleRegistry>();
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    var modules = registry.GetModulesStatus();
    return new { Version = version, Modules = modules };
});

app.Run();