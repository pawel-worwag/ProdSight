using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.UpdateFolder;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

public static class UpdateFolder
{
    public sealed record Request(Guid Id,UpdateFolderRequest Req) : IRequest<Folder>;

    public sealed class Handler(IFoldersRepository foldersRepository) :IRequestHandler<Request, Folder>
    {
        public async Task<Folder> HandleAsync(Request query, CancellationToken ct = default)
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

            return new Folder()
            {
                Id = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name,
                CreatedAt = folder.CreatedAt,
                Description = folder.Description
            };
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/documents/folders/{id}",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Update a folder")
                .Produces<Shared.DTOs.DocumentsModule.Folders.UpdateFolder.Folder>()
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, Folder> handler, Guid id,
            UpdateFolderRequest req, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(id,req), ct);
            return Results.Ok(result);
        }
    }
}