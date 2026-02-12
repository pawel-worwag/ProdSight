using ProdSight.Api;
using ProdSight.Api.Extensions;
using ProdSight.Api.Services;
using ProdSight.Api.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks(); 
builder.Services.AddModules(builder.Configuration);
builder.Services.AddSingleton<HealthCheckCacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<HealthCheckCacheService>());
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.AddScalar();

var app = builder.Build();

app.UseModules(builder.Configuration);
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapStatusEndpoint();
app.MapHealthEndpoint();
app.UseScalar();
app.UseMetricServer();

app.Run();