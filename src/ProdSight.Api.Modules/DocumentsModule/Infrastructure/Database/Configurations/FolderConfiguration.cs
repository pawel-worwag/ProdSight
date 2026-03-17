using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Configurations;

public class FolderConfiguration: IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasOne<Folder>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false);

        b.HasIndex(x => x.ParentId);
    }
}