using Microsoft.AspNetCore.Components;
using ProdSight.Api.Shared.DTOs.DocumentsModule.GetFolderDetails;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Pages.Documents;

public partial class Documents(IApiBroker api, NavigationManager nav) : ComponentBase
{
    [Parameter]
    public Guid? Id { get; init; }
    
    private List<ViewItem>? _items;
    private FolderDetails? _folderDetails;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        try
        {
            if (Id is not null)
            {
                _folderDetails = await api.GetFolderDetailsAsync(Id.Value);
                _items =
                [
                    new ViewItem()
                    {
                        Id = _folderDetails.ParentId,
                        IsFolder = true,
                        Name = "[..]"
                    }

                ];
                _items.AddRange( MapItems(await api.GetChildrenFoldersAsync(Id.Value)));
                
            }
            else
            {
                _folderDetails = null;
                _items = MapItems(await api.GetRootFoldersAsync());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void OnItemClick(bool isFolder, Guid? id)
    {
        if (isFolder)
        {
            if (id is null)
            {
                nav.NavigateTo($"/documents"); 
            }
            else
            {
                nav.NavigateTo($"/documents/{id}");
            }
            
        }
    }
    
    private record ViewItem
    {
        public Guid? Id { get; init; }
        public bool IsFolder { get; init; } = true;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public DateTimeOffset? CreatedAt  { get; init; }
    }

    private static List<ViewItem> MapItems(
        ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder> folders)
    {
        return folders.Select(p=>new ViewItem()
        {
            Id = p.Id,
            IsFolder = true,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
    private static List<ViewItem> MapItems(
        ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren.Folder> folders)
    {
        return folders.Select(p=>new ViewItem()
        {
            Id = p.Id,
            IsFolder = true,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}
