using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SpecificTopicMessageHandler : IIntegrationEventHandler<SpecificTopicOneMessageEvent>, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public SpecificTopicMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicOneMessageEvent message, CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicTwoMessageEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}