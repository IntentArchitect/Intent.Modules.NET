using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateRoots;
using FastEndpointsTest.Application.AggregateRoots.SearchAggregateRoots;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateRoots
{
    public class SearchAggregateRootsQueryEndpoint : Endpoint<SearchAggregateRootsQuery, List<AggregateRootDto>>
    {
        private readonly ISender _mediator;

        public SearchAggregateRootsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Query("api/aggregate-roots/search");
            Description(b =>
            {
                b.WithTags("AggregateRoots");
                b.WithSummary(@"HTTP QUERY endpoint: the search criteria travel in the request body.");
                b.Accepts<SearchAggregateRootsQuery>(MediaTypeNames.Application.Json);
                b.Produces<List<AggregateRootDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(SearchAggregateRootsQuery req, CancellationToken ct)
        {
            var result = default(List<AggregateRootDto>);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}