using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder
{
    /// <summary>
    /// Ships an order and publishes OrderShippedEvent. Exercises the publish path end to end.
    /// </summary>
    public class ShipOrderCommand : ICommand
    {
        public ShipOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}