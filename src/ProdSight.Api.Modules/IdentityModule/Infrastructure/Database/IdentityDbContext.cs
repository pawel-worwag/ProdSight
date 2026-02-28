namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Domain;
using Configurations;


public class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    IOptions<DatabaseOptions> dbOptions
    ): DbContext(options)
{
    private readonly DatabaseOptions _dbOptions = dbOptions.Value;

    public DbSet<UserSettings> UserSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_dbOptions.DatabaseSchema ?? "public");
        modelBuilder.ApplyConfiguration(new UserSettingsConfiguration());
    }
}