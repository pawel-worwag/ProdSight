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
using ProdSight.Api.Shared.DTOs.DocumentsModule.GetFolderDetails;
using ProdSight.Api.Shared.DTOs.Errors;
using RootFolderDto = ProdSight.Api.Shared.DTOs.DocumentsModule.GetRootFolders.Folder;
using CreateFolderDto = ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.Folder;
using CreateFolderRequestDto = ProdSight.Api.Shared.DTOs.DocumentsModule.CreateFolder.CreateFolderRequest;
using GetChildrenFolderDto = ProdSight.Api.Shared.DTOs.DocumentsModule.GetChildren.Folder;

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
        services.AddScoped<GetFolderDetailsHandler>();
        
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
            .WithTags("Documents Module")
            .Produces<IReadOnlyList<RootFolderDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        v1.MapPost("/documents/folders", async (CreateFolderHandler handler,
                CreateFolderRequestDto req, CancellationToken ct) =>
            {
                var created = await handler.HandleAsync(req, ct);
                return Results.Created($"/v1/documents/folders/{created.Id}", created);
            })
            .WithTags("Documents Module")
            .Produces<CreateFolderDto>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        v1.MapGet("/documents/folders/{parentId}/children",
            async (GetChildrenFoldersHandler handler, string parentId, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(Guid.Parse(parentId), ct)))
            .WithTags("Documents Module")
            .Produces<IReadOnlyList<GetChildrenFolderDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        v1.MapGet("/documents/folders/{id}/details",
            async (GetFolderDetailsHandler handler, string id, CancellationToken ct) =>
                Results.Ok(await handler.HandleAsync(Guid.Parse(id), ct)))
            .WithTags("Documents Module")
            .Produces<FolderDetails>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }
}