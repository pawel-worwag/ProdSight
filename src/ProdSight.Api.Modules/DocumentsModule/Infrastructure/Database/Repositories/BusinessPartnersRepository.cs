using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

public class BusinessPartnersRepository(DocumentsDbContext dbc)
    : IBusinessPartnersRepository
{
    public async Task<IReadOnlyCollection<BusinessPartner>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbc.BusinessPartners
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }
}