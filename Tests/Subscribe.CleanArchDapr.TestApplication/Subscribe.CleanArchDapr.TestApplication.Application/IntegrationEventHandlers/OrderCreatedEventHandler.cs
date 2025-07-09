using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.Publish.CleanArchDapr.TestApplication.Services.Orders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventHandler", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly IMyProxy _myProxy;

        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler(IMyProxy myProxy)
        {
            _myProxy = myProxy;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            await _myProxy.OrderConfirmedAsync(message.Id, new OrderConfirmed() { RefNo = "Bob", Id = message.Id }, cancellationToken);

        }
    }
}