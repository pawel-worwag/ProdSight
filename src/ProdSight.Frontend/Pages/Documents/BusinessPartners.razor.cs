using Microsoft.AspNetCore.Components;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Pages.Documents;

public partial class BusinessPartners(IApiBroker api) : ComponentBase
{
    private ICollection<ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.
        BusinessPartner>? _items;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _items = await api.GetAllBusinessPartnersAsync();
    }
}