using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn;
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
    public class UpdateAggregateTestNoIdReturnCommandEndpoint : Endpoint<UpdateAggregateTestNoIdReturnCommand>
    {
        private readonly ISender _mediator;

        public UpdateAggregateTestNoIdReturnCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/aggregate-test-no-id-returns/{id}");
            Description(b =>
            {
                b.WithTags("AggregateTestNoIdReturns");
                b.Accepts<UpdateAggregateTestNoIdReturnCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(UpdateAggregateTestNoIdReturnCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}