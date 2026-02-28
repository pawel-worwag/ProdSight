namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.IdentityModule.Domain;
using ProdSight.Api.Modules.IdentityModule.Infrastructure.Database.Configurations;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<UserSettings> UserSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserSettingsConfiguration());
    }
}
