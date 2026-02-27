using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ProdSight.Frontend.Pages.Identity;

public partial class Users : ComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; }
    private IJSObjectReference? _module;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./js/Identity/Users.js"
            );

        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}