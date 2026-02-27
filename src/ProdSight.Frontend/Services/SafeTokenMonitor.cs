using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Services;

public class SafeTokenMonitor : IAsyncDisposable
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly AuthenticationStateProvider _auth;
    private readonly string[] _scopes;
    private readonly JwtSecurityTokenHandler _jwt = new();
    private readonly ILogger<SafeTokenMonitor> _logger;
    private readonly CancellationTokenSource _cts = new();
    private Task? _backgroundTask;
    private DateTime? _expiresUtc;
    private bool _hasToken;

    public event Action? OnTick;
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(10);

    public SafeTokenMonitor(IAccessTokenProvider tokenProvider, IOptions<ApiOptions> opts, ILogger<SafeTokenMonitor> logger, AuthenticationStateProvider auth)
    {
        _tokenProvider = tokenProvider;
        _scopes = opts?.Value?.Scopes ?? Array.Empty<string>();
        _logger = logger;
        _auth = auth;
        Start();
    }

    private void Start()
    {
        if (_backgroundTask != null) return;
        _backgroundTask = Task.Run(() => RunAsync(_cts.Token), CancellationToken.None);
        _backgroundTask.ContinueWith(t =>
        {
            _logger.LogError(t.Exception, "SafeTokenMonitor background task faulted");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private async Task RunAsync(CancellationToken ct)
    {
        try
        {
            // We'll tick UI every second, and refresh token info only every PollInterval.
            var refreshIntervalSeconds = Math.Max(1, (int)PollInterval.TotalSeconds);
            var counter = 0;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (counter == 0)
                    {
                        // do actual token info refresh
                        await RefreshTokenInfoAsync(ct);
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while refreshing token info (ignored)");
                }

                // Notify UI every second
                OnTick?.Invoke();

                // wait one second
                await Task.Delay(TimeSpan.FromSeconds(1), ct);

                counter = (counter + 1) % refreshIntervalSeconds;
            }
        }
        catch (OperationCanceledException) { /* expected on dispose */ }
    }

    private async Task RefreshTokenInfoAsync(CancellationToken ct)
    {
        // avoid interactive redirect from background: check auth state first
        try
        {
            var authState = await _auth.GetAuthenticationStateAsync();
            if (authState.User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogDebug("User not authenticated - skipping token check");
                _expiresUtc = null;
                _hasToken = false;
                return;
            }

            var res = _scopes.Length > 0
                ? await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = _scopes })
                : await _tokenProvider.RequestAccessToken();

            _logger.LogInformation("RequestAccessToken result: {Status}", res.Status);

            if (!res.TryGetToken(out var at))
            {
                _logger.LogInformation("No access token available (TryGetToken returned false).");
                _expiresUtc = null;
                _hasToken = false;
                return;
            }

            _hasToken = true;
            var value = at.Value;
            _logger.LogInformation("Access token obtained, length={Length}", value?.Length ?? 0);
            if (value?.Split('.').Length == 3)
            {
                var jwt = _jwt.ReadJwtToken(value);
                _expiresUtc = jwt.ValidTo;
                _logger.LogInformation("Parsed JWT valid to {ExpUtc}", _expiresUtc);
            }
            else
            {
                _expiresUtc = null; // opaque token - we have token but no exp
                _logger.LogInformation("Token appears to be opaque (no JWT). Expires unknown.");
            }
        }
        catch (AccessTokenNotAvailableException ex)
        {
            _logger.LogInformation("Access token not available while polling (interactive required): {Message}", ex.Message);
            _expiresUtc = null;
            _hasToken = false;
            return;
        }

        // optional: try silent refresh here if near expiry
    }

    public TimeSpan? TimeLeft => _expiresUtc.HasValue ? _expiresUtc.Value - DateTime.UtcNow : null;
    public DateTime? ExpiresUtc => _expiresUtc;
    public bool HasToken => _hasToken;

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_backgroundTask != null)
        {
            try
            {
                var finished = await Task.WhenAny(_backgroundTask, Task.Delay(TimeSpan.FromSeconds(5)));
                if (finished != _backgroundTask) _logger.LogWarning("SafeTokenMonitor did not stop in time");
                await _backgroundTask; // propagate if faulted
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { _logger.LogError(ex, "Error disposing SafeTokenMonitor"); }
        }
        _cts.Dispose();
    }
}
