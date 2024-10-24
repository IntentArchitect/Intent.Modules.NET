using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongById;
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
    public class GetAggregateRootLongByIdQueryEndpoint : Endpoint<GetAggregateRootLongByIdQuery, AggregateRootLongDto>
    {
        private readonly ISender _mediator;

        public GetAggregateRootLongByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-root-longs/{id}");
            Description(b =>
            {
                b.WithTags("AggregateRootLongs");
                b.Accepts<GetAggregateRootLongByIdQuery>();
                b.Produces<AggregateRootLongDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetAggregateRootLongByIdQuery req, CancellationToken ct)
        {
            var result = default(AggregateRootLongDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}