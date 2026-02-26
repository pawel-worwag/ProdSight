using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak
{
    public class KeycloakApiBroker(
        HttpClient httpClient,
        IOptions<KeycloakOptions> options) : IKeycloakApiBroker
    {
        private string? _accessToken;
        private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;
        private readonly SemaphoreSlim _tokenLock = new(1, 1);

        // Shared, thread-safe JsonSerializerOptions for reuse
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<string> GetClientTokenAsync(CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(_accessToken) &&
                DateTimeOffset.UtcNow < _accessTokenExpiresAt.AddSeconds(-30))
            {
                return _accessToken;
            }

            await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Double-check after acquiring the semaphore:
                // Another thread may have refreshed the token while we were waiting
                // on `_tokenLock`. Therefore, we must re-check the cached token and
                // its expiry here — do not rely on semaphore state (e.g. `CurrentCount`)
                // because reading it can be subject to a race condition.
                if (!string.IsNullOrEmpty(_accessToken) &&
                    DateTimeOffset.UtcNow < _accessTokenExpiresAt.AddSeconds(-30))
                {
                    return _accessToken;
                }

                var tokenUrl = $"realms/{options.Value.Realm}/protocol/openid-connect/token";

                var form = new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = options.Value.ClientId,
                    ["client_secret"] = options.Value.ClientSecret
                };

                using var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                req.Content = new FormUrlEncodedContent(form);

                var res = await httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
                res.EnsureSuccessStatusCode();

                var tokenResp = await res.Content.ReadFromJsonAsync<TokenResponseDto>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (tokenResp?.AccessToken is null)
                    throw new InvalidOperationException("Keycloak response missing access_token");

                _accessToken = tokenResp.AccessToken;
                _accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResp.ExpiresIn);
                
                return _accessToken;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public async Task<IEnumerable<UserRepresentation>> GetUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var token = await GetClientTokenAsync(cancellationToken).ConfigureAwait(false);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var usersUrl = $"admin/realms/{options.Value.Realm}/users";

            var res = await httpClient.GetAsync(usersUrl, cancellationToken).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();

            var users = await res.Content.ReadFromJsonAsync<List<UserRepresentation>>(JsonOptions, cancellationToken)
                .ConfigureAwait(false);

            return users ?? [];
        }
    }
}