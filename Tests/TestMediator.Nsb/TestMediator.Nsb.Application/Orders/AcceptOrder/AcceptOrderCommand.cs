using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Nsb.Application.Orders.AcceptOrder
{
    public class AcceptOrderCommand : ICommand
    {
        public AcceptOrderCommand(Guid orderId, string customerName)
        {
            OrderId = orderId;
            CustomerName = customerName;
        }

        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
    }
}