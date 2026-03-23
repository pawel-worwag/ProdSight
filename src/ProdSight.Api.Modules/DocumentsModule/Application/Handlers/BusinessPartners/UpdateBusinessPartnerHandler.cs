using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.UpdateBusinessPartner;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;

public class UpdateBusinessPartnerHandler(IBusinessPartnersRepository repo)
{
    public async Task<BusinessPartner> HandleAsync(Guid id, UpdateBusinessPartnerRequest req,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
        {
            throw new BadRequestException("Name is required");
        }

        var bp = await repo.GetAsync(id, ct);
        
        if (bp is null)
        {
            throw new BadRequestException("Not found");
        }
        
        bp.ChangeName(req.Name);
        bp.ChangeAddress(req.Address);
        bp.ChangeDescription(req.Description);

        await repo.SaveChangesAsync(ct);
        return new BusinessPartner()
        {
            Id = bp.Id,
            Name = bp.Name,
            Address = bp.Address,
            Description = bp.Description,
            CreatedAt = bp.CreatedAt
        };
    }
}