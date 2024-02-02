using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers.Test
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AnotherTestMessageHandler : IIntegrationEventHandler<AnotherTestMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public AnotherTestMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(AnotherTestMessageEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}