using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Transport.AmazonSqs.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : ICommand
    {
        public CreateOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}