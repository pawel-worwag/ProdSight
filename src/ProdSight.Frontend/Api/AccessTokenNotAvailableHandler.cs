using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ProdSight.Frontend.Api;

public class AccessTokenNotAvailableHandler(NavigationManager nav) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (AccessTokenNotAvailableException ex)
        {
            var relative = nav.ToBaseRelativePath(nav.Uri);
            var returnUrl = string.IsNullOrEmpty(relative) ? "/" : "/" + relative; // upewnij się, że ma leading slash
            var encoded = Uri.EscapeDataString(returnUrl);
            nav.NavigateTo($"reauthrequired?returnUrl={encoded}", forceLoad: true);
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized) { RequestMessage = request };

        }
    }
}