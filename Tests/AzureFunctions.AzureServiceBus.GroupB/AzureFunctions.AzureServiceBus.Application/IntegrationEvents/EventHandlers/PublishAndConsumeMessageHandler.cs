using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PublishAndConsumeMessageHandler : IIntegrationEventHandler<PublishAndConsumeMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public PublishAndConsumeMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(PublishAndConsumeMessageEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (PublishAndConsumeMessageHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}