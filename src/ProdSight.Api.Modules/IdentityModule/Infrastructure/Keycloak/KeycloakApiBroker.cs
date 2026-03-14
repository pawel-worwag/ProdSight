using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak
{
    public class KeycloakApiBroker(
        IHttpClientFactory httpClientFactory,
        IKeycloakTokenProvider tokenProvider,
        IOptions<KeycloakOptions> options) : IKeycloakApiBroker
    {
        // Shared, thread-safe JsonSerializerOptions for reuse
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private Uri PrepareUri(HttpClient httpClient,string relative)
        {
            return httpClient.BaseAddress is not null
                ? new Uri(httpClient.BaseAddress, relative)
                : new Uri(relative, UriKind.Relative);
        }

        public async Task<IEnumerable<UserRepresentation>> GetUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var httpClient = httpClientFactory.CreateClient("keycloak");
            var token = await tokenProvider.GetClientTokenAsync(cancellationToken).ConfigureAwait(false);
            var requestUri = PrepareUri(httpClient,$"admin/realms/{options.Value.Realm}/users");
            using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();

            var users = await res.Content.ReadFromJsonAsync<List<UserRepresentation>>(JsonOptions, cancellationToken)
                .ConfigureAwait(false);

            return users ?? [];
        }

        public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
        {
            var httpClient = httpClientFactory.CreateClient("keycloak");
            var requestUri = PrepareUri(httpClient,$"realms/{options.Value.Realm}/.well-known/openid-configuration");

            var res = await httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Response is {res.StatusCode}");
            }

            var discovery = await res.Content.ReadFromJsonAsync<DiscoveryResponse>(JsonOptions, cancellationToken)
                .ConfigureAwait(false);
            if (discovery is null)
            {
                return false;
            }

            return (!string.IsNullOrWhiteSpace(discovery.TokenEndpoint) &&
                    !string.IsNullOrWhiteSpace(discovery.JwksUri));
        }
    }
}