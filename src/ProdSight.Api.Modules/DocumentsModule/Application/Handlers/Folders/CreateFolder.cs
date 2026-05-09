using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.Folders.CreateFolder;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.Folders;

public static class CreateFolder
{
    public sealed record Request(string Name, string? Description, Guid? ParentId) 
        : IRequest<Folder>;

    public sealed class Handler(IFoldersRepository foldersRepository) : IRequestHandler<Request, Folder>
    {
        public async Task<Folder> HandleAsync(Request query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query.Name))
                throw new BadRequestException("Name is required");
            var domainFolder = Domain.Entities.Folder.Create(query.Name.Trim(), query.Description, query.ParentId);
            var created = await foldersRepository.CreateAsync(domainFolder, ct);
            return new Folder
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreatedAt = created.CreatedAt
            };
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/documents/folders",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Folders"])
                .WithSummary("Create a new folder")
                .Produces<Folder>(StatusCodes
                    .Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, Folder> handler, CreateFolderRequest req, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(req.Name,req.Description,req.ParentId), ct);
            return Results.Created($"/v1/documents/folders/{result.Id}", result);
        }
    }
}

