using System.Net.Mime;
using CqrsSpike.Subscriber.Application.Shipments;
using CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CqrsSpike.Subscriber.Api.FastEndpoints.Shipments
{
    public class GetShipmentByOrderIdQueryEndpoint : Endpoint<GetShipmentByOrderIdQuery, ShipmentDto>
    {
        private readonly ISender _mediator;

        public GetShipmentByOrderIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/shipments/by-order/{orderId}");
            Description(b =>
            {
                b.WithTags("Shipments");
                b.Accepts<GetShipmentByOrderIdQuery>();
                b.Produces<ShipmentDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetShipmentByOrderIdQuery req, CancellationToken ct)
        {
            var result = default(ShipmentDto);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}