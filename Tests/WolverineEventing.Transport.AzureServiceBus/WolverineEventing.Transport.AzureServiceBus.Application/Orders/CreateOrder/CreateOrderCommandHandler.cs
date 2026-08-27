using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Transport.AzureServiceBus.Application.Common.Eventing;
using WolverineEventing.Transport.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace WolverineEventing.Transport.AzureServiceBus.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Publish(new OrderCreatedEvent
            {
            });
        }
    }
}