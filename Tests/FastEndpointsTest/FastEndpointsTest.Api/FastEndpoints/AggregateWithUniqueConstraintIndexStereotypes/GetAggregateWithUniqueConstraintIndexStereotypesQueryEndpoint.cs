using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.GetAggregateWithUniqueConstraintIndexStereotypes;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateWithUniqueConstraintIndexStereotypes
{
    public class GetAggregateWithUniqueConstraintIndexStereotypesQueryEndpoint : EndpointWithoutRequest<List<AggregateWithUniqueConstraintIndexStereotypeDto>>
    {
        private readonly ISender _mediator;

        public GetAggregateWithUniqueConstraintIndexStereotypesQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-with-unique-constraint-index-stereotypes");
            Description(b =>
            {
                b.WithTags("AggregateWithUniqueConstraintIndexStereotypes");
                b.Produces<List<AggregateWithUniqueConstraintIndexStereotypeDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<AggregateWithUniqueConstraintIndexStereotypeDto>);
            result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexStereotypesQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}