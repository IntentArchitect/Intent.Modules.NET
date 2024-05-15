using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.Publish.CleanArchDapr.TestApplication.Services.Orders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventHandlerImplementation", Version = "2.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderCreatedEventHandler : IRequestHandler<OrderCreatedEvent>
    {
        private readonly IMyProxy _myProxy;

        [IntentManaged(Mode.Ignore)]
        public OrderCreatedEventHandler(IMyProxy myProxy)
        {
            _myProxy = myProxy;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            await _myProxy.OrderConfirmedAsync(@event.Id, new OrderConfirmed() { RefNo = "Bob", Id = @event.Id }, cancellationToken);

        }
    }
}