using System.Net.Mime;
using CqrsSpike.Subscriber.Application.OrderRecords;
using CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CqrsSpike.Subscriber.Api.FastEndpoints.OrderRecords
{
    public class GetOrderRecordByOrderIdQueryEndpoint : Endpoint<GetOrderRecordByOrderIdQuery, OrderRecordDto>
    {
        private readonly ISender _mediator;

        public GetOrderRecordByOrderIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/order-records/{orderId}");
            Description(b =>
            {
                b.WithTags("OrderRecords");
                b.Accepts<GetOrderRecordByOrderIdQuery>();
                b.Produces<OrderRecordDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetOrderRecordByOrderIdQuery req, CancellationToken ct)
        {
            var result = default(OrderRecordDto);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}