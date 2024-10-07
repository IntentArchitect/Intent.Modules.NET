using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypes;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.UniqueIndexConstraint.ClassicMapping
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
            Get("api/unique-index-constraint-stereotype");
            Description(b =>
            {
                b.WithTags("UniqueIndexConstraintClassicMapping");
                b.Produces<List<AggregateWithUniqueConstraintIndexStereotypeDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<AggregateWithUniqueConstraintIndexStereotypeDto>);
            result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexStereotypesQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}