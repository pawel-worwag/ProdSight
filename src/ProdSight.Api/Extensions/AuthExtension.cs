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
                var metadata = config["Oidc:MetadataAddress"] ?? throw new InvalidOperationException("Configuration 'Oidc:MetadataAddress' is not set.");
                var authority = config["Oidc:Authority"] ?? throw new InvalidOperationException("Configuration 'Oidc:Authority' is not set.");
                var audience = config["Oidc:Audience"] ?? throw new InvalidOperationException("Configuration 'Oidc:Audience' is not set.");

                options.MetadataAddress = metadata;
                options.RequireHttpsMetadata = true;
                options.MapInboundClaims = true;
                options.Authority = authority;
                options.Audience = audience;
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