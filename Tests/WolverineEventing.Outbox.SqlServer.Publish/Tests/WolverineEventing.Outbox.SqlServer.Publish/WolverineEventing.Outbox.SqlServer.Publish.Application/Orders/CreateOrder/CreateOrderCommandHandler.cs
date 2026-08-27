using Intent.RoslynWeaver.Attributes;
using MediatR;
using WolverineEventing.Outbox.SqlServer.Publish.Application.Common.Eventing;
using WolverineEventing.Outbox.SqlServer.Publish.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WolverineEventing.Outbox.SqlServer.Publish.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
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