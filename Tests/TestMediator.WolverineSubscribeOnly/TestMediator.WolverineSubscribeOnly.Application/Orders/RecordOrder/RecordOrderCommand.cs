using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Application.Orders.RecordOrder
{
    public class RecordOrderCommand : ICommand
    {
        public RecordOrderCommand(Guid orderId, string customerName)
        {
            OrderId = orderId;
            CustomerName = customerName;
        }

        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
    }
}