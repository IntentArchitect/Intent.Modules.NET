using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateTestNoIdReturns
{
    public class GetAggregateTestNoIdReturnsQueryEndpoint : EndpointWithoutRequest<List<AggregateTestNoIdReturnDto>>
    {
        private readonly ISender _mediator;

        public GetAggregateTestNoIdReturnsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-test-no-id-returns");
            Description(b =>
            {
                b.WithTags("AggregateTestNoIdReturns");
                b.Produces<List<AggregateTestNoIdReturnDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<AggregateTestNoIdReturnDto>);
            result = await _mediator.Send(new GetAggregateTestNoIdReturnsQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}