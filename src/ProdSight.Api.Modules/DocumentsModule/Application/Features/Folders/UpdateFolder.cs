using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

/// <summary>
/// Feature slice responsible for updating an existing folder.
/// </summary>
public static class UpdateFolder
{
    public sealed record Request(Guid Id,DTOs.UpdateFolderRequest Req) : IRequest<DTOs.Folder>;

    /// <summary>
    /// Handles the business logic for updating a folder.
    /// </summary>
    public sealed class Handler(IFoldersRepository foldersRepository) :IRequestHandler<Request, DTOs.Folder>
    {
        public async Task<DTOs.Folder> HandleAsync(Request query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query.Req.Name))
            {
                throw new BadRequestException("Name is required");
            }

            var folder = await foldersRepository.GetAsync(query.Id, ct);

            if (folder is null)
            {
                throw new BadRequestException("Not found");
            }
        
            folder.Rename(query.Req.Name);
            folder.ChangeDescription(query.Req.Description);
            folder.ChangeParent(query.Req.ParentId);
            await foldersRepository.SaveChangesAsync(ct);

            return new DTOs.Folder()
            {
                Id = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name,
                CreatedAt = folder.CreatedAt,
                Description = folder.Description
            };
        }
    }

    /// <summary>
    /// Exposes the HTTP endpoint for updating a folder.
    /// </summary>
    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/documents/folders/{id}",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Update a folder")
                .Produces<DTOs.Folder>()
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, DTOs.Folder> handler, Guid id,
            DTOs.UpdateFolderRequest req, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(id,req), ct);
            return Results.Ok(result);
        }
    }
}
