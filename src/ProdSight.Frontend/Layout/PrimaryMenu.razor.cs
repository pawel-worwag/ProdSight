using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ProdSight.Frontend.Layout;

public partial class PrimaryMenu : ComponentBase, IDisposable
{
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Parameter]
    public Func<string, Task>? NavigateAsync { get; set; }

    private ClaimsPrincipal? User { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthStateProvider.GetAuthenticationStateAsync();
        User = state.User;
        AuthStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public async Task OnNavigateAsync(string path)
    {
        if (NavigateAsync is not null)
        {
            await NavigateAsync(path);
            return;
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _ = HandleStateChangeAsync(task);
    }

    private async Task HandleStateChangeAsync(Task<AuthenticationState> task)
    {
        var state = await AuthStateProvider.GetAuthenticationStateAsync();
        User = state.User;
        StateHasChanged();
    }

    public void Dispose()
    {
        try
        {
            AuthStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
        catch
        {
            // ignore
        }
    }
}
