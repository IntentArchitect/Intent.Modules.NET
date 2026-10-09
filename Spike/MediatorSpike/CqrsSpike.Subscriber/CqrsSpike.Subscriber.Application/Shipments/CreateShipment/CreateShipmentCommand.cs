using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    public class CreateShipmentCommand : ICommand<Guid>
    {
        public CreateShipmentCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}