using System.Net.Mime;
using CqrsSpike.Subscriber.Application.Shipments.CreateShipment;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CqrsSpike.Subscriber.Api.FastEndpoints.Shipments
{
    public class CreateShipmentCommandEndpoint : Endpoint<CreateShipmentCommand, Guid>
    {
        private readonly ISender _mediator;

        public CreateShipmentCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/shipments");
            Description(b =>
            {
                b.WithTags("Shipments");
                b.Accepts<CreateShipmentCommand>(MediaTypeNames.Application.Json);
                b.Produces<Guid>(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateShipmentCommand req, CancellationToken ct)
        {
            var result = Guid.Empty;
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Created(string.Empty, result));
        }
    }
}