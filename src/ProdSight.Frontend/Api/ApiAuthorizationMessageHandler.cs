using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace ProdSight.Frontend.Api;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    private readonly IAccessTokenProvider _provider;
    private readonly ILogger<ApiAuthorizationMessageHandler> _logger;
    private DateTimeOffset? _expires=null;
    private readonly TokenMonitor? _tokenMonitor = null;

    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigation,
        IOptions<ApiOptions> opts,
        ILogger<ApiAuthorizationMessageHandler> logger,
        TokenMonitor monitor)
        : base(provider, navigation)
    {
        _logger = logger;
        _provider = provider;
        _tokenMonitor = monitor;
        
        var o = opts.Value;
        ConfigureHandler(authorizedUrls: [o.BaseUrl], o.Scopes);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var res = await _provider.RequestAccessToken();
            
            _logger.LogInformation("RequestAccessToken status: {Status}", res.Status);
            
            if (res.TryGetToken(out var at))
            {
                if (_expires != at.Expires)
                {
                    _expires = at.Expires;
                    _logger.LogInformation("New access token expires: {Expires}", _expires);
                    
                    _tokenMonitor?.SetExpires(_expires);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to get access token");
        }
        return await base.SendAsync(request, cancellationToken);
    }
}