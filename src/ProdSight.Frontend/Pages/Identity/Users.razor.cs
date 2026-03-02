using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Pages.Identity;

public partial class Users : ComponentBase
{
    [Inject]
    private IJSRuntime? Js { get; set; }
    private IJSObjectReference? _module;

    [Inject]
    private IApiBroker? Api { get; set; }
    
    private ICollection<User>? Data { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Js is not null)
            {
                _module = await Js.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./js/Identity/Users.js"
                );
            }
            
            await LoadData();
        }
    }

    private async Task LoadData()
    {
        if (Api is not null)
        {
            try
            {
                Data = await Api.GetAllUsersAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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