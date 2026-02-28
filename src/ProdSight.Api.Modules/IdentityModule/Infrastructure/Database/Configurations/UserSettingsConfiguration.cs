namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Database.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdSight.Api.Modules.IdentityModule.Domain;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.ToTable("UserSettings");
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.TimeZoneId).IsRequired().HasMaxLength(100);
    }
}
