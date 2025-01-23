using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Scheduled;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DelayedNotificationEventHandler : IIntegrationEventHandler<DelayedNotificationEvent>
    {
        [IntentManaged(Mode.Merge)]
        public DelayedNotificationEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(DelayedNotificationEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}