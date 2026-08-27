using Intent.RoslynWeaver.Attributes;
using MediatR;
using WolverineEventing.Outbox.SqlServer.Publish.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WolverineEventing.Outbox.SqlServer.Publish.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest, ICommand
    {
        public CreateOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}