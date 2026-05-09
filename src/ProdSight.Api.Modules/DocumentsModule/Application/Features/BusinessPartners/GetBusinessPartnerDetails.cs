using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetBusinessPartnerDetails;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.BusinessPartners;

public static class GetBusinessPartnerDetails
{
    public sealed record Request(Guid Id):IRequest<DTOs.BusinessPartnerDetails>;

    public sealed class Handler(IBusinessPartnersRepository repo) : IRequestHandler<Request, DTOs.BusinessPartnerDetails>
    {
        public async Task<DTOs.BusinessPartnerDetails> HandleAsync(Request query, CancellationToken ct = default)
        {
            var bp = await repo.GetAsync(query.Id, ct);
            if (bp is null)
            {
                var ex = new NotFoundException("BP not found");
                ex.Data.Add("id", query.Id);
                throw ex;
            }
            return new DTOs.BusinessPartnerDetails()
            {
                Id = bp.Id,
                Name = bp.Name,
                Address = bp.Address,
                Description = bp.Description,
                CreatedAt = bp.CreatedAt
            };
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/v1/documents/business-partners/{id}/details",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Business Partners"])
                .WithSummary("Get business partner details")
                .Produces<DTOs.BusinessPartnerDetails>(
                    StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, DTOs.BusinessPartnerDetails> handler,
            string id, CancellationToken ct = default)
        {
            var result = await handler.HandleAsync(new Request(Guid.Parse(id)), ct);
            return Results.Ok(result);
        }
    }
}