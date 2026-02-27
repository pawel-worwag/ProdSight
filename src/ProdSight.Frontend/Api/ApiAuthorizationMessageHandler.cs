using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace ProdSight.Frontend.Api;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation,IOptions<ApiOptions> opts) : base(provider, navigation)
    {
        var o = opts.Value;
        ConfigureHandler(
            authorizedUrls: [o.BaseUrl],Array.Empty<string>());
    }
}