using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateTestNoIdReturns
{
    public class DeleteAggregateTestNoIdReturnCommandEndpoint : Endpoint<DeleteAggregateTestNoIdReturnCommand>
    {
        private readonly ISender _mediator;

        public DeleteAggregateTestNoIdReturnCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Delete("api/aggregate-test-no-id-returns/{id}");
            Description(b =>
            {
                b.WithTags("AggregateTestNoIdReturns");
                b.Accepts<DeleteAggregateTestNoIdReturnCommand>();
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(DeleteAggregateTestNoIdReturnCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok());
        }
    }
}