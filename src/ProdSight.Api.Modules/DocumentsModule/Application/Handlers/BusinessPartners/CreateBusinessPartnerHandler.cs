using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.CreateBusinessPartner;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;

public class CreateBusinessPartnerHandler(IBusinessPartnersRepository repo)
{
    public async Task<BusinessPartner> HandleAsync(CreateBusinessPartnerRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new BadRequestException("Name is required");
        var bp = Domain.Entities.BusinessPartner.Create(req.Name, req.Address, req.Description);
        var created = await repo.CreateAsync(bp, ct);

        return new BusinessPartner()
        {
            Id = created.Id,
            Address = created.Address,
            Name = created.Name,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        };
    }
}