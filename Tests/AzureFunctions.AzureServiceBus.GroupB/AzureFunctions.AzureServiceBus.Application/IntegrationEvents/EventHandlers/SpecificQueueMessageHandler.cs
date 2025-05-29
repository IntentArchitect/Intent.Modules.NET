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
    public class SpecificQueueMessageHandler : IIntegrationEventHandler<SpecificQueueOneMessageEvent>, IIntegrationEventHandler<SpecificQueueTwoMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public SpecificQueueMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(SpecificQueueOneMessageEvent message, CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(SpecificQueueTwoMessageEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}