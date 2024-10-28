using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.GetAggregateWithUniqueConstraintIndexStereotypeById;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateWithUniqueConstraintIndexStereotypes
{
    public class GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryEndpoint : Endpoint<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery, AggregateWithUniqueConstraintIndexStereotypeDto>
    {
        private readonly ISender _mediator;

        public GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/aggregate-with-unique-constraint-index-stereotypes/{id}");
            Description(b =>
            {
                b.WithTags("AggregateWithUniqueConstraintIndexStereotypes");
                b.Accepts<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery>();
                b.Produces<AggregateWithUniqueConstraintIndexStereotypeDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(
            GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery req,
            CancellationToken ct)
        {
            var result = default(AggregateWithUniqueConstraintIndexStereotypeDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}