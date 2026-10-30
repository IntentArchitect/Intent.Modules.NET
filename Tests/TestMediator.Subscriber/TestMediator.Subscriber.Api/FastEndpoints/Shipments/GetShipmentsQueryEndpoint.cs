using System.Net.Mime;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Subscriber.Application.Shipments;
using TestMediator.Subscriber.Application.Shipments.GetShipments;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace TestMediator.Subscriber.Api.FastEndpoints.Shipments
{
    public class GetShipmentsQueryEndpoint : EndpointWithoutRequest<List<ShipmentDto>>
    {
        private readonly ISender _sender;

        public GetShipmentsQueryEndpoint(ISender sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
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
            result = await _sender.Send(new GetShipmentsQuery(), ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}