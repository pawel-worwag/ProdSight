using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.IdentityModule.Application.Keycloak.DTOs;

namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Keycloak;

public class KeycloakTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakOptions> options) : IKeycloakTokenProvider
{
    private string? _accessToken;
    private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private Uri PrepareUri(HttpClient httpClient,string relative)
    {
        return httpClient.BaseAddress is not null
            ? new Uri(httpClient.BaseAddress, relative)
            : new Uri(relative, UriKind.Relative);
    }

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

            var httpClient = httpClientFactory.CreateClient("keycloak");
            var requestUri = PrepareUri(httpClient,$"realms/{options.Value.Realm}/protocol/openid-connect/token");

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = options.Value.ClientId,
                ["client_secret"] = options.Value.ClientSecret
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, requestUri);
            req.Content = new FormUrlEncodedContent(form);

            var res = await httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();

            var tokenResp = await res.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions, cancellationToken)
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
}