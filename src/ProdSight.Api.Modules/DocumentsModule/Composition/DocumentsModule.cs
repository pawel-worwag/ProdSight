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

        services.AddScoped<IFoldersRepository, FoldersRepository>();
        services.AddScoped<IBusinessPartnersRepository, BusinessPartnersRepository>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("documents-module-database-check", tags: ["documents-module"]);
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapApiEndpoints(ProdSight.Api.Modules.DocumentsModule.Application.AssemblyMarker.Assembly);
    }
    
}