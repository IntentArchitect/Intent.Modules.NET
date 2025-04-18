using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateRoots;
using FastEndpointsTest.Application.AggregateRoots.GetAggregateRootCompositeManyBS;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateRoots
{
    public class GetAggregateRootCompositeManyBSQueryEndpoint : Endpoint<GetAggregateRootCompositeManyBSQuery, List<AggregateRootCompositeManyBDto>>
    {
        private readonly ISender _mediator;

        public GetAggregateRootCompositeManyBSQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-roots/composite-many-b/{aggregateRootId}");
            Description(b =>
            {
                b.WithTags("AggregateRoots");
                b.Accepts<GetAggregateRootCompositeManyBSQuery>();
                b.Produces<List<AggregateRootCompositeManyBDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(GetAggregateRootCompositeManyBSQuery req, CancellationToken ct)
        {
            var result = default(List<AggregateRootCompositeManyBDto>);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}