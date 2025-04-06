using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ClientCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}