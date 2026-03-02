using ProdSight.Api;
using ProdSight.Api.Extensions;
using ProdSight.Api.Services;
using ProdSight.Api.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddHealthChecks();
builder.Services.AddModules(configuration);
builder.Services.AddSingleton<HealthCheckCacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<HealthCheckCacheService>());
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.AddScalar();



builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddJWTAuth(builder.Configuration);


var app = builder.Build();

app.UseRouting();
app.UseCors("ApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();


app.UseModules(configuration);
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapStatusEndpoint();
app.MapHealthEndpoint();
app.UseScalar();
app.UseMetricServer();

app.Run();