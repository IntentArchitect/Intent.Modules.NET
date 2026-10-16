using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Subscriber.Application.Shipments.CompleteShipment
{
    public class CompleteShipmentCommand : ICommand
    {
        public CompleteShipmentCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}