using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.DocumentsModule.Composition.HealthChecks;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;
using ProdSight.Api.Shared.Modules;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;
using ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;
using ProdSight.Api.Shared.DTOs.Errors;
using Folder = ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder.Folder;

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
        services.AddScoped<GetChildrenFoldersHandler>();
        services.AddScoped<GetFoldersDetailsHandler>();
        services.AddScoped<GetFoldersTreeHandler>();
        services.AddScoped<UpdateFolderHandler>();
        
        services.AddScoped<GetAllBusinessPartnersHandler>();
        
        services.AddScoped<IFoldersRepository, FoldersRepository>();
        services.AddScoped<IBusinessPartnersRepository, BusinessPartnersRepository>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("documents-module-database-check", tags: ["documents-module"]);
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        var v1 = endpoints.MapGroup("v1");
        v1.MapGet("/documents", () => "Documents module endpoint")
            .WithTags("Documents Module");
        
        ConfigureFoldersEndpoints(v1);
        ConfigureBusinessPartnersEndpoints(v1);
    }
    
    private void ConfigureFoldersEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/documents/folders/root", async (GetRootFoldersHandler handler, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(ct)))
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.Folders.GetRootFolders.Folder>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        
        endpoints.MapGet("/documents/folders/tree", async (GetFoldersTreeHandler handler, CancellationToken ct)=>
            Results.Ok(await handler.HandleAsync(ct)))
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.Folders.GetFoldersTree.Folder>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapPost("/documents/folders", async (CreateFolderHandler handler,
                CreateFolderRequest req, CancellationToken ct) =>
            {
                var created = await handler.HandleAsync(req, ct);
                return Results.Created($"/v1/documents/folders/{created.Id}", created);
            })
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<Folder>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapGet("/documents/folders/{parentId}/children",
            async (GetChildrenFoldersHandler handler, string parentId, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(Guid.Parse(parentId), ct)))
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.Folders.GetChildren.Folder>>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapGet("/documents/folders/{id}/details",
            async (GetFoldersDetailsHandler handler, string id, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(Guid.Parse(id), ct)))
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<FolderDetails>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        
        endpoints.MapPost("/documents/folders/{id}", async (UpdateFolderHandler handler, string id,
            UpdateFolderRequest req, CancellationToken ct) =>
            {
                var updated = await handler.HandleAsync(Guid.Parse(id), req, ct);
                return Results.Ok(updated);
            })
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<Shared.DTOs.DocumentsModule.Folders.UpdateFolder.Folder>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }

    private void ConfigureBusinessPartnersEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/documents/bp/root", async (GetAllBusinessPartnersHandler handler, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(ct)))
            .WithTags(["Documents Module", "Documents Module - Business Partners"])
            .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }
}