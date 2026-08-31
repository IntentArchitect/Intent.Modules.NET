using Intent.RoslynWeaver.Attributes;
using Wolverine;
using WolverineEventing.Coexist.Cqrs.Application.Common.Eventing;
using WolverineEventing.Coexist.Cqrs.Application.Orders.GetExistingOrder;
using WolverineEventing.Coexist.Cqrs.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler
    {
        private readonly IMessageBus _messageBus;
        private readonly Wolverine.IMessageBus _sender;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IMessageBus messageBus, Wolverine.IMessageBus sender)
        {
            _messageBus = messageBus;
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }


        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var query = new GetExistingOrderQuery(
                orderId: request.OrderId);
            var orderDto = await _sender.InvokeAsync<OrderDto>(query, cancellationToken);
            _messageBus.Publish(new OrderCreatedEvent
            {
                OrderId = request.OrderId
            });
        }
    }
}