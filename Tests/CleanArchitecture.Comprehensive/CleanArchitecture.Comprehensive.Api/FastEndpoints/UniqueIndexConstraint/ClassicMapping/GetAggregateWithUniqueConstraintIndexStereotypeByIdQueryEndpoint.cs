using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypeById;
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
    public class GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryEndpoint : Endpoint<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery, AggregateWithUniqueConstraintIndexStereotypeDto>
    {
        private readonly ISender _mediator;

        public GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/unique-index-constraint-stereotype/{id}");
            Description(b =>
            {
                b.WithTags("UniqueIndexConstraintClassicMapping");
                b.Accepts<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery>();
                b.Produces<AggregateWithUniqueConstraintIndexStereotypeDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
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