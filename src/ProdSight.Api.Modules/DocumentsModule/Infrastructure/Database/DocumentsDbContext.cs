using Microsoft.EntityFrameworkCore;
using ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database;

public class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) 
    : DbContext(options)
{
    public DbSet<Folder> Folders => Set<Folder>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}