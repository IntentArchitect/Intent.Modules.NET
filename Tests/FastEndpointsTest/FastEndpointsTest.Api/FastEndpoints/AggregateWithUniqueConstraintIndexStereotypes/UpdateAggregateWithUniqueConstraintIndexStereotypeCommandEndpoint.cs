using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.UpdateAggregateWithUniqueConstraintIndexStereotype;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateWithUniqueConstraintIndexStereotypes
{
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint : Endpoint<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly ISender _mediator;

        public UpdateAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/aggregate-with-unique-constraint-index-stereotypes/{id}");
            Description(b =>
            {
                b.WithTags("AggregateWithUniqueConstraintIndexStereotypes");
                b.Accepts<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(
            UpdateAggregateWithUniqueConstraintIndexStereotypeCommand req,
            CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}