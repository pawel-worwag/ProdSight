using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database;

// Simpler design-time factory: use ConfigurationBuilder + Get<T>() to load
// `DocumentsModule:Database` section and construct the DbContext.
public class DocumentsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DocumentsDbContext>
{
    public DocumentsDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var basePath = AppContext.BaseDirectory;

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var dbOptions = config.GetSection("DocumentsModule:Database").Get<DatabaseOptions>();

        if (dbOptions is null || string.IsNullOrWhiteSpace(dbOptions.DatabaseSchema) || string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
        {
            throw new InvalidOperationException("Appsettings error: ConnectionString and DatabaseSchema must be set for design-time operations and migrations.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DocumentsDbContext>();
        optionsBuilder.UseNpgsql(dbOptions.ConnectionString, npgsql =>
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory", dbOptions.DatabaseSchema));

        var options = Options.Create(dbOptions);
        return new DocumentsDbContext(optionsBuilder.Options, options);
    }
}
