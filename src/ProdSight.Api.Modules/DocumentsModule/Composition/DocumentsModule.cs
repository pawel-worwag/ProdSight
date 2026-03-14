using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdSight.Api.Shared.Modules;

namespace ProdSight.Api.Modules.DocumentsModule.Composition;

public class DocumentsModule : IModule
{
    public string Name => "DocumentsModule";
    public string RequiredScope => "documents-module";

    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        // minimal: no services to register
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/documents", () => "Documents module endpoint")
            .WithTags("Documents Module");
    }
}
