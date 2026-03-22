using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Configurations;

public class BusinessPartnerConfiguration: IEntityTypeConfiguration<BusinessPartner>
{
    public void Configure(EntityTypeBuilder<BusinessPartner> b)
    {
        b.ToTable("BusinessPartners");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x=>x.CreatedAt).IsRequired();
    }
}