using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

public static class GetChildrenFolders
{
    public sealed record Request(Guid ParentId) : IRequest<IReadOnlyList<DTOs.Folder>>;

    public sealed class Handler(IFoldersRepository foldersRepository)
        : IRequestHandler<Request, IReadOnlyList<DTOs.Folder>>
    {
        public async Task<IReadOnlyList<DTOs.Folder>> HandleAsync(Request query, CancellationToken ct = default)
        {
            var folders = await foldersRepository.GetChildrenAsync(query.ParentId, ct);
            
            return folders.Select(f => new DTOs.Folder
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                CreatedAt = f.CreatedAt
            }).ToList();
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/v1/documents/folders/{parentId}/children",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Get children folders")
                .Produces<IReadOnlyList<DTOs.Folder>>()
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, IReadOnlyList<DTOs.Folder>> handler,string parentId, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(Guid.Parse(parentId)), ct);
            return Results.Ok(result);
        }
    }
}

