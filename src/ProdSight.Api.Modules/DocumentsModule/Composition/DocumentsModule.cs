using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Modules.DocumentsModule.Application;
using ProdSight.Api.Modules.DocumentsModule.Composition.HealthChecks;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Extensions;
using ProdSight.Api.Shared.Modules;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;
using ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;
using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Repositories;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Composition;

public class DocumentsModule : IModule
{
    public string Name => "DocumentsModule";
    public string RequiredScope => "documents-module";

    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddDocumentsDatabase(config);
        services.AddRequestHandlersFromAssembly(ProdSight.Api.Modules.DocumentsModule.Application.AssemblyMarker.Assembly);
        
        services.AddScoped<CreateFolderHandler>();
        services.AddScoped<GetChildrenFoldersHandler>();
        services.AddScoped<GetFoldersDetailsHandler>();
        services.AddScoped<UpdateFolderHandler>();

        services.AddScoped<GetAllBusinessPartnersHandler>();
        services.AddScoped<CreateBusinessPartnerHandler>();
        services.AddScoped<UpdateBusinessPartnerHandler>();
        services.AddScoped<GetBusinessPartnerDetailsHandler>();

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

        endpoints.MapApiEndpoints(ProdSight.Api.Modules.DocumentsModule.Application.AssemblyMarker.Assembly);
    }

    private void ConfigureFoldersEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/documents/folders",
                async (CreateFolderHandler handler,
                    ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder.CreateFolderRequest req,
                    CancellationToken ct) =>
                {
                    var created = await handler.HandleAsync(req, ct);
                    return Results.Created($"/v1/documents/folders/{created.Id}", created);
                })
            .WithTags(["Documents Module", "Documents Module - Folders"])
            .Produces<ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder.Folder>(StatusCodes
                .Status201Created)
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
            .Produces<ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails.FolderDetails>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapPost("/documents/folders/{id}",
                async (UpdateFolderHandler handler, string id,
                    ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder.UpdateFolderRequest req,
                    CancellationToken ct) =>
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
        endpoints.MapGet("/documents/business-partners",
                async (GetAllBusinessPartnersHandler handler, CancellationToken ct) =>
                    Results.Ok(await handler.HandleAsync(ct)))
            .WithTags(["Documents Module", "Documents Module - Business Partners"])
            .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapPost("/documents/business-partners",
                async (CreateBusinessPartnerHandler handler,
                    ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.CreateBusinessPartner.
                        CreateBusinessPartnerRequest req, CancellationToken ct) =>
                {
                    var created = await handler.HandleAsync(req, ct);
                    return Results.Created($"/documents/business-partners/{created.Id}", created);
                })
            .WithTags(["Documents Module", "Documents Module - Business Partners"])
            .Produces<Shared.DTOs.DocumentsModule.BusinessPartners.CreateBusinessPartner.BusinessPartner>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapPost("/documents/business-partners/{id}",
                async (Guid id,
                    ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.UpdateBusinessPartner.
                        UpdateBusinessPartnerRequest req, UpdateBusinessPartnerHandler handler, CancellationToken ct) =>
                {
                    var bp = await handler.HandleAsync(id, req, ct);
                    return Results.Ok(bp);
                })
            .WithTags(["Documents Module", "Documents Module - Business Partners"])
            .Produces<Shared.DTOs.DocumentsModule.BusinessPartners.UpdateBusinessPartner.BusinessPartner>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");

        endpoints.MapGet("/documents/business-partners/{id}/details",
                async (Guid id, GetBusinessPartnerDetailsHandler handler, CancellationToken ct) => Results.Ok(await handler.HandleAsync(id, ct)))
            .WithTags(["Documents Module", "Documents Module - Business Partners"])
            .Produces<Shared.DTOs.DocumentsModule.BusinessPartners.GetBusinessPartnerDetails.BusinessPartnerDetails>(
                StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }
}