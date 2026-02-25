using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak;

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
    }
}
