using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.UpdateBusinessPartner;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;
using ProdSight.Api.Shared.Messaging;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Handlers.BusinessPartners;

public static class UpdateBusinessPartner
{
    public sealed record Request(Guid Id, UpdateBusinessPartnerRequest Req):IRequest<BusinessPartner>;

    public sealed class Handler(IBusinessPartnersRepository repo) : IRequestHandler<Request, BusinessPartner>
    {
        public async Task<BusinessPartner> HandleAsync(Request query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query.Req.Name))
            {
                throw new BadRequestException("Name is required");
            }

            var bp = await repo.GetAsync(query.Id, ct);
        
            if (bp is null)
            {
                throw new BadRequestException("Not found");
            }
        
            bp.ChangeName(query.Req.Name);
            bp.ChangeAddress(query.Req.Address);
            bp.ChangeDescription(query.Req.Description);

            await repo.SaveChangesAsync(ct);
            return new BusinessPartner()
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
            endpoints.MapPost("/v1/documents/business-partners/{id}",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Business Partners"])
                .WithSummary("Update a business partner")
                .Produces<Shared.DTOs.DocumentsModule.BusinessPartners.UpdateBusinessPartner.BusinessPartner>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, BusinessPartner> handler,Request query, CancellationToken ct)
        {
            var result = await handler.HandleAsync(query, ct);
            return Results.Ok(result);
        }
    }
}