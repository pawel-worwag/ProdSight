using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace ProdSight.Frontend.Api;

public class UserProfileService : IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(AuthenticationStateProvider authProvider, ILogger<UserProfileService> logger)
    {
        _authProvider = authProvider;
        _logger = logger;
        _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _ = InitialLogAsync();
    }

    private async Task InitialLogAsync()
    {
        try
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            LogState(state.User?.Identity?.IsAuthenticated == true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get initial authentication state");
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _ = HandleStateChangeAsync(task);
    }

    private async Task HandleStateChangeAsync(Task<AuthenticationState> task)
    {
        try
        {
            var state = await task.ConfigureAwait(false);
            var isAuth = state.User?.Identity?.IsAuthenticated == true;
            LogState(isAuth);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while handling authentication state change");
        }
    }

    private void LogState(bool signedIn)
    {
        if (signedIn)
        {
            _logger.LogInformation("User signed in (UserProfileService)");
        }
        else
        {
            _logger.LogInformation("User signed out (UserProfileService)");
        }
    }

    public void Dispose()
    {
        _authProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
