using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PublishAndConsumeEventHandler : IIntegrationEventHandler<PublishAndConsumeEvent>
    {
        [IntentManaged(Mode.Merge)]
        public PublishAndConsumeEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(PublishAndConsumeEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (PublishAndConsumeEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}