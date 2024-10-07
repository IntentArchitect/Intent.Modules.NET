using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.UpdateAggregateWithUniqueConstraintIndexStereotype;
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
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint : Endpoint<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly ISender _mediator;

        public UpdateAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/unique-index-constraint-stereotype/{id}");
            Description(b =>
            {
                b.WithTags("UniqueIndexConstraintClassicMapping");
                b.Accepts<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
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