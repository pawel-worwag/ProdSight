using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentsDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection("DocumentsModule:Database"));
        
        services.AddDbContextPool<DocumentsDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>();
                
            var connectionString = dbOptions.Value.ConnectionString;
            var schema = dbOptions.Value.DatabaseSchema  ?? "public";
                
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schema);
                // Retry up to 5 times with max delay 30s for transient failures
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            });
        });

        services.Configure<DiskFileStorageOptions>(configuration.GetSection("DocumentsModule:DiskFileStorage"));
        services.AddSingleton<IFileStorage, DiskFileStorage>();
        
        return services;
    }
}
