using AspNetCore.AzureServiceBus.GroupA.Eventing.Messages;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.IntegrationEventHandler", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ClientCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}