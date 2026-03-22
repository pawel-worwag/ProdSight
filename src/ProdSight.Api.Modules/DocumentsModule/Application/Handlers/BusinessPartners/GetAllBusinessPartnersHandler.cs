using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;


namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;

public class GetAllBusinessPartnersHandler(IBusinessPartnersRepository repo)
{
    public async Task<IReadOnlyCollection<ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner>> HandleAsync(CancellationToken ct = default)
    {
        return (await repo.GetAllAsync(ct)).Select(x => new ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner()
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            Description = x.Description,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}