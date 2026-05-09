using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.CreateBusinessPartner;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.BusinessPartners;

/// <summary>
/// Feature slice responsible for creating a business partner.
/// </summary>
public static class CreateBusinessPartner
{
    public sealed record Request(DTOs.CreateBusinessPartnerRequest Req):IRequest<DTOs.BusinessPartner>;

    /// <summary>
    /// Handles the business logic for creating a business partner.
    /// </summary>
    public sealed class Handler(IBusinessPartnersRepository repo) : IRequestHandler<Request, DTOs.BusinessPartner>
    {
        public async Task<DTOs.BusinessPartner> HandleAsync(Request query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query.Req.Name))
                throw new BadRequestException("Name is required");
            var bp = Domain.Entities.BusinessPartner.Create(query.Req.Name, query.Req.Address, query.Req.Description);
            var created = await repo.CreateAsync(bp, ct);

            return new DTOs.BusinessPartner()
            {
                Id = created.Id,
                Address = created.Address,
                Name = created.Name,
                Description = created.Description,
                CreatedAt = created.CreatedAt
            };
        }
    }

    /// <summary>
    /// Exposes the HTTP endpoint for creating a business partner.
    /// </summary>
    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/documents/business-partners",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Business Partners"])
                .WithSummary("Create a new business partner")
                .Produces<DTOs.BusinessPartner>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, DTOs.BusinessPartner> handler,DTOs.CreateBusinessPartnerRequest req,
            CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(req), ct);
            return Results.Created($"/v1/documents/business-partners/{result.Id}", result);
        }
    }
}
