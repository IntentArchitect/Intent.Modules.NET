using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.MyProxy;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventHandler", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderCreatedEventHandler : IRequestHandler<OrderCreatedEvent>
    {
        private readonly IMyProxyClient _myProxy;

        [IntentManaged(Mode.Ignore)]
        public OrderCreatedEventHandler(IMyProxyClient myProxy)
        {
            _myProxy = myProxy;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {

            await _myProxy.OrderConfirmedAsync(@event.Id, new OrderConfirmed() { RefNo = "Bob", Id = @event.Id }, cancellationToken);
            return Unit.Value;
        }
    }
}