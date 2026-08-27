using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Transport.AzureServiceBus.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace WolverineEventing.Transport.AzureServiceBus.Application.Orders.CreateOrder
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