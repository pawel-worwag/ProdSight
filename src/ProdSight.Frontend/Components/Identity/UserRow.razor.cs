using Microsoft.AspNetCore.Components;
using ProdSight.Api.Shared.DTOs.IdentityModule.GetUsersList;
namespace ProdSight.Frontend.Components.Identity;

public partial class UserRow : ComponentBase
{
    [Parameter]
    public required User User { get; set; }
}