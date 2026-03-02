using ProdSight.Api;
using ProdSight.Api.Extensions;
using ProdSight.Api.Services;
using ProdSight.Api.Middleware;
using Prometheus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = builder.Configuration["Oidc:MetadataAddress"]; 
        options.Authority = builder.Configuration["Oidc:Authority"];
        options.Audience = builder.Configuration["Oidc:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.Error.WriteLine($"Auth failed: {ctx.Exception?.Message}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();


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