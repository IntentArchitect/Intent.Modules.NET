using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers.NamingOverrides
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SubscriptionConsumerHandler : IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>
    {
        [IntentManaged(Mode.Merge)]
        public SubscriptionConsumerHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(
            StandardMessageCustomSubscribeEvent message,
            CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(
            OverrideMessageCustomSubscribeEvent message,
            CancellationToken cancellationToken = default)
        {
        }
    }
}