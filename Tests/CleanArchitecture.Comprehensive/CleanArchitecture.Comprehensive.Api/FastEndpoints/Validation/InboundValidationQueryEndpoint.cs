using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Validation;
using CleanArchitecture.Comprehensive.Application.Validation.InboundValidation;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.Validation
{
    public class InboundValidationQueryEndpoint : Endpoint<InboundValidationQuery, DummyResultDto>
    {
        private readonly ISender _mediator;

        public InboundValidationQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/validation/inbound-validation");
            Description(b =>
            {
                b.WithTags("Validation");
                b.Accepts<InboundValidationQuery>();
                b.Produces<DummyResultDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(InboundValidationQuery req, CancellationToken ct)
        {
            var result = default(DummyResultDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}