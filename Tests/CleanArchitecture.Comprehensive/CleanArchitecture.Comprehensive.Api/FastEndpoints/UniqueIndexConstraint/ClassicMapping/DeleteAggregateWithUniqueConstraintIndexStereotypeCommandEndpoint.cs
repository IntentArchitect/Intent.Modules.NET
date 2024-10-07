using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexStereotype;
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
    public class DeleteAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint : Endpoint<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly ISender _mediator;

        public DeleteAggregateWithUniqueConstraintIndexStereotypeCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Delete("api/unique-index-constraint-stereotype/{id}");
            Description(b =>
            {
                b.WithTags("UniqueIndexConstraintClassicMapping");
                b.Accepts<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>();
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(
            DeleteAggregateWithUniqueConstraintIndexStereotypeCommand req,
            CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok());
        }
    }
}