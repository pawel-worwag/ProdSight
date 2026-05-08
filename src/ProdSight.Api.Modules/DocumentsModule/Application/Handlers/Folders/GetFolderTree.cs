using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFoldersTree;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public static class GetFoldersTree
{
    public sealed record Request : IRequest<ICollection<Folder>>;

    public sealed class Handler(IFoldersRepository foldersRepository): IRequestHandler<Request, ICollection<Folder>>
    {
        public async Task<ICollection<Folder>> HandleAsync(Request query, CancellationToken ct = default)
        {
            var records = await foldersRepository.GetAllAsync(ct);
            
            var tree = records.ToDictionary(r => r.Id, r => new Folder()
            {
                Id = r.Id,
                ParentId = r.ParentId,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt,
                Children = new List<Folder>()
            });
            
            foreach (var t in tree)
            {
                if (t.Value.ParentId != null)
                {
                    var pId = t.Value.ParentId.Value;
                    tree[pId].Children.Add(t.Value);
                }
            }
        
            return tree.Where(f=>f.Value.ParentId == null)
                .Select(f => f.Value).ToList();
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/documents/folders/tree",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Get folders tree")
                .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.Folders.GetFoldersTree.Folder>>()
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, ICollection<Folder>> handler,CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(), ct);
            return Results.Ok(result);
        }
    }
}
