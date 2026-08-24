using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RequestOrderProcessingCommandHandler
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public RequestOrderProcessingCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(RequestOrderProcessingCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Send(new ProcessOrderCommand
            {
                OrderId = request.OrderId
            });
        }
    }
}