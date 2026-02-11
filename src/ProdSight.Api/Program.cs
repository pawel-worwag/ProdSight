using ProdSight.Api;
using ProdSight.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks(); 
builder.Services.AddModules(builder.Configuration);
builder.Services.AddSingleton<HealthCheckCacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<HealthCheckCacheService>());



var app = builder.Build();


app.UseModules(builder.Configuration);
app.MapStatusEndpoint();
app.MapHealthEndpoint();


app.Run();