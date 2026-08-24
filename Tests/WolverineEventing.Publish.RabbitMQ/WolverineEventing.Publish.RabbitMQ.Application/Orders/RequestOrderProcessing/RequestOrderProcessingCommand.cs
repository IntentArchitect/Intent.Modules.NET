using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing
{
    /// <summary>
    /// Requests order processing by sending the ProcessOrderCommand integration command.
    /// </summary>
    public class RequestOrderProcessingCommand : ICommand
    {
        public RequestOrderProcessingCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}