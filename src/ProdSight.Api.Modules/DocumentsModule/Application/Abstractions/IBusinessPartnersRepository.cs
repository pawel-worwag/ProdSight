using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;

public interface IBusinessPartnersRepository
{
    Task<IReadOnlyCollection<BusinessPartner>> GetAllAsync(CancellationToken ct = default);
    Task<BusinessPartner> CreateAsync(BusinessPartner bp, CancellationToken ct = default);
}