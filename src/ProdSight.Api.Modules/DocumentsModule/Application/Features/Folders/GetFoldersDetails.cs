using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.GetFolderDetails;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.Folders;

public static class GetFoldersDetails
{
    public sealed record Request(Guid Id):IRequest<FolderDetails>;

    public sealed class Handler(IFoldersRepository foldersRepository) : IRequestHandler<Request, FolderDetails>
    {
        public async Task<FolderDetails> HandleAsync(Request query, CancellationToken ct = default)
        {
            var folder = await foldersRepository.GetAsync(query.Id, ct);
            return new FolderDetails
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
                .Produces<FolderDetails>()
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, FolderDetails> handler, string id, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(Guid.Parse(id)), ct);
            return Results.Ok(result);
        }
    }
}