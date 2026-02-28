using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak;
using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeycloakBroker(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Keycloak");
            services.Configure<KeycloakOptions>(section);

            services.AddHttpClient<IKeycloakApiBroker, KeycloakApiBroker>((sp, client) =>
            {
                var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<KeycloakOptions>>().Value;
                if (!string.IsNullOrEmpty(opts.BaseUrl))
                    client.BaseAddress = new Uri(opts.BaseUrl);
            });

            return services;
        }

        public static IServiceCollection AddIdentityDatabase(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "IdentityModule")
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{connectionStringName}' is not configured.");

            // Use pooled DbContext for better throughput and configure retry on failure for transient PostgreSQL errors
            services.AddDbContextPool<IdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Retry up to 5 times with max delay 30s for transient failures
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                });
            });

            return services;
        }
    }
}
