using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    public class GetShipmentByOrderIdQuery : IQuery<ShipmentDto>
    {
        public GetShipmentByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}