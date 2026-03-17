using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database;

// Design-time factory ensures EF tools create the DbContext with the same
// DatabaseOptions (including DatabaseSchema) as at runtime.
public class DocumentsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DocumentsDbContext>
{
    public DocumentsDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var basePath = AppContext.BaseDirectory;
        var dbOptions = new DatabaseOptions();

        // Try to read environment-specific appsettings first, fallback to appsettings.json
        var envPath = Path.Combine(basePath, $"appsettings.{env}.json");
        var defaultPath = Path.Combine(basePath, "appsettings.json");
        var jsonPath = File.Exists(envPath) ? envPath : (File.Exists(defaultPath) ? defaultPath : null);

        if (jsonPath != null)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));
                if (doc.RootElement.TryGetProperty("DocumentsModule", out var dm) && dm.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (dm.TryGetProperty("Database", out var db) && db.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        if (db.TryGetProperty("ConnectionString", out var cs)) dbOptions.ConnectionString = cs.GetString();
                        if (db.TryGetProperty("DatabaseSchema", out var ds)) dbOptions.DatabaseSchema = ds.GetString();
                    }
                }
            }
            catch
            {
                // ignore parse errors and fall back to defaults
            }
        }

        var optionsBuilder = new DbContextOptionsBuilder<DocumentsDbContext>();
        optionsBuilder.UseNpgsql(dbOptions.ConnectionString ?? string.Empty, npgsql =>
        {
            var schema = dbOptions.DatabaseSchema ?? "public";
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema);
        });

        var options = Options.Create(dbOptions);
        return new DocumentsDbContext(optionsBuilder.Options, options);
    }
}
