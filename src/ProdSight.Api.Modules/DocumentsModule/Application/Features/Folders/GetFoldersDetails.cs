using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

public static class GetFoldersDetails
{
    public sealed record Request(Guid Id):IRequest<DTOs.FolderDetails>;

    public sealed class Handler(IFoldersRepository foldersRepository) : IRequestHandler<Request, DTOs.FolderDetails>
    {
        public async Task<DTOs.FolderDetails> HandleAsync(Request query, CancellationToken ct = default)
        {
            var folder = await foldersRepository.GetAsync(query.Id, ct);
            return new DTOs.FolderDetails
            {
                Id = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name,
                Description = folder.Description,
                CreatedAt = folder.CreatedAt
            };
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/v1/documents/folders/{d}/details",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Get folder details")
                .Produces<DTOs.FolderDetails>()
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, DTOs.FolderDetails> handler, string id, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(Guid.Parse(id)), ct);
            return Results.Ok(result);
        }
    }
}