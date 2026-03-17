using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProdSight.Api.Modules.IdentityModule.Infrastructure.Database;

// Design-time factory for IdentityDbContext — reads IdentityModule:Database config,
// validates ConnectionString and DatabaseSchema, and constructs the DbContext.
public class IdentityDesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var basePath = AppContext.BaseDirectory;

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var dbOptions = config.GetSection("IdentityModule:Database").Get<DatabaseOptions>() ?? new DatabaseOptions();

        if (string.IsNullOrWhiteSpace(dbOptions.DatabaseSchema) || string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
        {
            throw new InvalidOperationException("Configuration error: ConnectionString and DatabaseSchema must be set for design-time operations and migrations.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(dbOptions.ConnectionString, npgsql =>
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory", dbOptions.DatabaseSchema));

        var options = Microsoft.Extensions.Options.Options.Create(dbOptions);
        return new IdentityDbContext(optionsBuilder.Options, options);
    }
}
