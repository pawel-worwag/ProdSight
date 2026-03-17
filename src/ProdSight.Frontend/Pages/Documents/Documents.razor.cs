using Microsoft.AspNetCore.Components;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Pages.Documents;

public partial class Documents(IApiBroker api) : ComponentBase
{
    private ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder>? _rootFolders;
    
    protected async override Task OnInitializedAsync()
    {
        await  base.OnInitializedAsync();
        try
        {
            _rootFolders = await api.GetRootFoldersAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}