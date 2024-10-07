using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateRootLongs
{
    public class DeleteAggregateRootLongCommandEndpoint : Endpoint<DeleteAggregateRootLongCommand>
    {
        private readonly ISender _mediator;

        public DeleteAggregateRootLongCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Delete("api/aggregate-root-longs/{id}");
            Description(b =>
            {
                b.WithTags("AggregateRootLongs");
                b.Accepts<DeleteAggregateRootLongCommand>();
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(DeleteAggregateRootLongCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok());
        }
    }
}