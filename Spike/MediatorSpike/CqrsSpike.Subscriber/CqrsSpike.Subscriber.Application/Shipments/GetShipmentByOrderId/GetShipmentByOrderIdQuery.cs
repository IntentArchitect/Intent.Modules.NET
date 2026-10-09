using CqrsSpike.Subscriber.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    public class GetShipmentByOrderIdQuery : IRequest<ShipmentDto>, IQuery
    {
        public GetShipmentByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}