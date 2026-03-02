using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ProdSight.Api.Extensions;

public static class AuthExtension
{
    public static IServiceCollection AddJWTAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MetadataAddress = config["Oidc:MetadataAddress"]; 
                options.RequireHttpsMetadata = true;
                options.MapInboundClaims = true;
                options.Authority = config["Oidc:Authority"];
                options.Audience = config["Oidc:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.Error.WriteLine($"Auth failed: {ctx.Exception?.Message}");
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }

}