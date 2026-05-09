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
        
        services.AddScoped<CreateBusinessPartnerHandler>();

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
        
        ConfigureBusinessPartnersEndpoints(v1);

        endpoints.MapApiEndpoints(ProdSight.Api.Modules.DocumentsModule.Application.AssemblyMarker.Assembly);
    }

    private void ConfigureBusinessPartnersEndpoints(IEndpointRouteBuilder endpoints)
    {
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
    }
}