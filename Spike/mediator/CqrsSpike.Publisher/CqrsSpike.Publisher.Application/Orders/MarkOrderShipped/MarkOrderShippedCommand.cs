using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

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