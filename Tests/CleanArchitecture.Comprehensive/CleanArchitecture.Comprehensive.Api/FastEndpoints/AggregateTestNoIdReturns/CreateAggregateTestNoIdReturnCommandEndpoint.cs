using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn;
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
    public class CreateAggregateTestNoIdReturnCommandEndpoint : Endpoint<CreateAggregateTestNoIdReturnCommand>
    {
        private readonly ISender _mediator;

        public CreateAggregateTestNoIdReturnCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/aggregate-test-no-id-returns");
            Description(b =>
            {
                b.WithTags("AggregateTestNoIdReturns");
                b.Accepts<CreateAggregateTestNoIdReturnCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CreateAggregateTestNoIdReturnCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}