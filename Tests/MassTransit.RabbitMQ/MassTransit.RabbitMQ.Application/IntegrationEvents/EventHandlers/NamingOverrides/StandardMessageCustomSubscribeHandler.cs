using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Application.NamingOverrides.SendFromEventHandler;
using MassTransit.RabbitMQ.Services.NamingOverrides;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers.NamingOverrides
{
    [IntentManaged(Mode.Fully, Body = Mode.Fully)]
    public class StandardMessageCustomSubscribeHandler : IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>
    {
        private readonly ISender _mediator;
        [IntentManaged(Mode.Fully)]
        public StandardMessageCustomSubscribeHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(
            StandardMessageCustomSubscribeEvent message,
            CancellationToken cancellationToken = default)
        {
            var command = new SendFromEventHandlerCommand(message: message.Message);

            await _mediator.Send(command, cancellationToken);
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