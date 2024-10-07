using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRoots;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateRoots
{
    public class GetAggregateRootsQueryEndpoint : EndpointWithoutRequest<List<AggregateRootDto>>
    {
        private readonly ISender _mediator;

        public GetAggregateRootsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-roots");
            Description(b =>
            {
                b.WithTags("AggregateRoots");
                b.Produces<List<AggregateRootDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<AggregateRootDto>);
            result = await _mediator.Send(new GetAggregateRootsQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}