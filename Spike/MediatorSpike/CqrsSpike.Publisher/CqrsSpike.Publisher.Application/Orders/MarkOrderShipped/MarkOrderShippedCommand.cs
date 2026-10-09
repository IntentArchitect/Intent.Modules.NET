using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    public class MarkOrderShippedCommand : ICommand
    {
        public MarkOrderShippedCommand(Guid orderId, Guid shipmentId)
        {
            OrderId = orderId;
            ShipmentId = shipmentId;
        }

        public Guid OrderId { get; set; }
        public Guid ShipmentId { get; set; }
    }
}