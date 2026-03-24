using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetBusinessPartnerDetails;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;

public class GetBusinessPartnerDetailsHandler(IBusinessPartnersRepository repo)
{
    public async
        Task<BusinessPartnerDetails> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var bp = await repo.GetAsync(id, ct);
        if (bp is null)
        {
            var ex = new NotFoundException("BP not found");
            ex.Data.Add("id", id);
            throw ex;
        }
        return new BusinessPartnerDetails()
        {
            Id = bp.Id,
            Name = bp.Name,
            Address = bp.Address,
            Description = bp.Description,
            CreatedAt = bp.CreatedAt
        };
    }
}