using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.EventDomain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler()
        {
        }

        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}