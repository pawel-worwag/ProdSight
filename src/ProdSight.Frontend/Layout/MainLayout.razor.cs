using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ProdSight.Frontend.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject]
    private IJSRuntime Js { get; set; } = default!;
    private ElementReference _offcanvasElement;
    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _module = await Js.InvokeAsync<IJSObjectReference>("import", "./js/offcanvas.js");
            }
            catch
            {
                // Nie przerywamy renderowania jeśli import się nie powiedzie;
            }
        }
    }

    private async Task OpenMenuAsync()
    {
        if (_module is null) return;
        await _module.InvokeAsync<bool>("open", _offcanvasElement);
    }

    private async Task CloseMenuAsync()
    {
        if (_module is null) return;
        await _module.InvokeAsync<bool>("close", _offcanvasElement);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
