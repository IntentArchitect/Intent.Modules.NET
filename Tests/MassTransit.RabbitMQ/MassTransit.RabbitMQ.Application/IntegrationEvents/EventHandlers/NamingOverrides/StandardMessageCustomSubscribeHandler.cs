using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers.NamingOverrides
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StandardMessageCustomSubscribeHandler : IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>
    {
        [IntentManaged(Mode.Merge)]
        public StandardMessageCustomSubscribeHandler()
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
            OverrideMessageStandardSubscribeEvent message,
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