using Intent.RoslynWeaver.Attributes;
using MediatR;
using WolverineEventing.ErrorPolicy.ScheduleRetry.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.ScheduleRetry.Application.Orders.CreateOrder
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