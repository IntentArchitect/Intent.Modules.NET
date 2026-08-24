using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.FailOrder
{
    /// <summary>
    /// Publishes FailingOrderEvent. Exists only to drive the retry/dead-letter path.
    /// </summary>
    public class FailOrderCommand : ICommand
    {
        public FailOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}