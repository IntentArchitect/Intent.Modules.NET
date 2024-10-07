using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateRootLongs
{
    public class GetAggregateRootLongsQueryEndpoint : Endpoint<GetAggregateRootLongsQuery, PagedResult<AggregateRootLongDto>>
    {
        private readonly ISender _mediator;

        public GetAggregateRootLongsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-root-longs");
            Description(b =>
            {
                b.WithTags("AggregateRootLongs");
                b.Accepts<GetAggregateRootLongsQuery>();
                b.Produces<PagedResult<AggregateRootLongDto>>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetAggregateRootLongsQuery req, CancellationToken ct)
        {
            var result = default(PagedResult<AggregateRootLongDto>);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}