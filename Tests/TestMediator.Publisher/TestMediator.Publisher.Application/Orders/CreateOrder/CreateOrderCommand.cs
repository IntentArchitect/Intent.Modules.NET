using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public CreateOrderCommand(string customerName, decimal total)
        {
            CustomerName = customerName;
            Total = total;
        }

        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}