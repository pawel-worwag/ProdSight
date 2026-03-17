using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.DocumentsModule.Composition.HealthChecks;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;
using ProdSight.Api.Shared.Modules;
using ProdSight.Api.Modules.DocumentsModule.Application.Handlers;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;

namespace ProdSight.Api.Modules.DocumentsModule.Composition;

public class DocumentsModule : IModule
{
    public string Name => "DocumentsModule";
    public string RequiredScope => "documents-module";

    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddDocumentsDatabase(config);
        services.AddScoped<GetRootFoldersHandler>();
        services.AddScoped<CreateFolderHandler>();
        services.AddScoped<IFoldersRepository, FoldersRepository>();
        
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("documents-module-database-check", tags: ["documents-module"]);
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        var v1 = endpoints.MapGroup("v1");
        
        v1.MapGet("/documents", () => "Documents module endpoint")
            .WithTags("Documents Module");

        v1.MapGet("/documents/folders/root", async (GetRootFoldersHandler handler, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(ct)))
            .WithTags("Documents Module");

        v1.MapPost("/documents/folders", async (CreateFolderHandler handler, ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.CreateFolderRequest req, CancellationToken ct) =>
                {
                    var created = await handler.HandleAsync(req, ct);
                    return Results.Created($"/v1/documents/folders/{created.Id}", created);
                })
            .WithTags("Documents Module");
    }
}
