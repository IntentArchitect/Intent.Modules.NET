using System.Net.Mime;
using CqrsSpike.Subscriber.Application.Shipments.GetShipments;
using CqrsSpike.Subscriber.Application.Shipments;
using FastEndpoints;
using Mediator;

namespace CqrsSpike.Subscriber.Api.FastEndpoints.Shipments
{
    public class GetShipmentsQueryEndpoint : EndpointWithoutRequest<List<ShipmentDto>>
    {
        private readonly ISender _mediator;

        public GetShipmentsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/shipments");
            Description(b =>
            {
                b.WithTags("Shipments");
                b.Produces<List<ShipmentDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<ShipmentDto>);
            result = await _mediator.Send(new GetShipmentsQuery(), ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}