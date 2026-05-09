using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetRootFolders;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

/// <summary>
/// Feature slice responsible for listing root folders.
/// </summary>
public static class GetRootFolders
{
    public sealed record Request:IRequest<IReadOnlyList<DTOs.Folder>>;

    /// <summary>
    /// Handles the business logic for retrieving root folders.
    /// </summary>
    public sealed class Handler(IFoldersRepository foldersRepository) : IRequestHandler<Request, IReadOnlyList<DTOs.Folder>>
    {
        public async Task<IReadOnlyList<DTOs.Folder>> HandleAsync(Request query, CancellationToken ct = default)
        {
            var folders = await foldersRepository.GetRootsAsync(ct);
            return folders.Select(f => new DTOs.Folder
            {
                Id = f.Id,
                Name = f.Name,
                CreatedAt = f.CreatedAt
            }).ToList();
        }
    }

    /// <summary>
    /// Exposes the HTTP endpoint for listing root folders.
    /// </summary>
    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/v1/documents/folders/root/", GetAllAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Get root folders")
                .Produces<IReadOnlyList<DTOs.Folder>>()
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }
        
        private static async Task<IResult> GetAllAsync(
            IRequestHandler<Request, IReadOnlyList<DTOs.Folder>> handler,
            CancellationToken cancellationToken)
        {
            var dockerRegistries = await handler.HandleAsync(new Request(), cancellationToken);
            return Results.Ok(dockerRegistries);
        }
    }
}
