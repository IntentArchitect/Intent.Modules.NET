using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.PaginationForProxies.PaginatedResult;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.PaginationForProxies
{
    public class PaginatedResultQueryEndpoint : Endpoint<PaginatedResultQuery, PagedResult<string>>
    {
        private readonly ISender _mediator;

        public PaginatedResultQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/pagination-for-proxies/paginated-result");
            Description(b =>
            {
                b.WithTags("PaginationForProxies");
                b.Accepts<PaginatedResultQuery>();
                b.Produces<PagedResult<string>>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(PaginatedResultQuery req, CancellationToken ct)
        {
            var result = default(PagedResult<string>);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}