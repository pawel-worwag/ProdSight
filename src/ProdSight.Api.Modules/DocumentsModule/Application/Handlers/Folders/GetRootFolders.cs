using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public static class GetRootFolders
{
    public sealed record Request():IRequest<IReadOnlyList<Folder>>;

    public sealed class Handler(IFoldersRepository foldersRepository) : IRequestHandler<Request, IReadOnlyList<Folder>>
    {
        public async Task<IReadOnlyList<Folder>> HandleAsync(Request query, CancellationToken ct = default)
        {
            var folders = await foldersRepository.GetRootsAsync(ct);
            return folders.Select(f => new Folder
            {
                Id = f.Id,
                Name = f.Name,
                CreatedAt = f.CreatedAt
            }).ToList();
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/documents/folders/root/", GetAllAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Get root folders")
                .Produces<IReadOnlyList<Folder>>()
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }
        
        private static async Task<IResult> GetAllAsync(
            IRequestHandler<Request, IReadOnlyList<Folder>> handler,
            CancellationToken cancellationToken)
        {
            var dockerRegistries = await handler.HandleAsync(new Request(), cancellationToken);
            return Results.Ok(dockerRegistries);
        }
    }
}