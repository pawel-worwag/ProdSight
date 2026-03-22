using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database;

public class DocumentsDbContext : DbContext
{
    private readonly string _schema;

    public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options, IOptions<DatabaseOptions> dbOptions)
        : base(options)
    {
        _schema = dbOptions?.Value?.DatabaseSchema ?? "public";
    }

    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}