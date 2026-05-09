using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Modules.DocumentsModule.Application.Abstractions;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;
using DTOs = ProdSight.Api.Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners;

namespace ProdSight.Api.Modules.DocumentsModule.Application.Features.BusinessPartners;

public static class GetAllBusinessPartners
{
    public sealed record Request():IRequest<IReadOnlyCollection<DTOs.BusinessPartner>>;

    public sealed class Handler(IBusinessPartnersRepository repo) : IRequestHandler<Request, IReadOnlyCollection<DTOs.BusinessPartner>>
    {
        public async Task<IReadOnlyCollection<DTOs.BusinessPartner>> HandleAsync(Request query, CancellationToken ct = default)
        {
            return (await repo.GetAllAsync(ct)).Select(x => new DTOs.BusinessPartner()
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Description = x.Description,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
    }

    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/v1/documents/business-partners",ExecuteAsync)
                .WithTags(["Documents Module", "Documents Module - Business Partners"])
                .WithSummary("Get all business partners")
                .Produces<IReadOnlyList<Shared.DTOs.DocumentsModule.BusinessPartners.GetAllBusinessPartners.BusinessPartner>>()
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }

        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, IReadOnlyCollection<DTOs.BusinessPartner>> handler, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(), ct);
            return Results.Ok(result);
        }
    }
}