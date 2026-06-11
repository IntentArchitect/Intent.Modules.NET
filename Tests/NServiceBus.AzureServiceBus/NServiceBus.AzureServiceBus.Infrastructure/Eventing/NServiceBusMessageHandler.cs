using Intent.RoslynWeaver.Attributes;
using NServiceBus.AzureServiceBus.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageHandler", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Infrastructure.Eventing
{
    internal class NServiceBusMessageHandler<TMessage> : IHandleMessages<TMessage>
        where TMessage : class
    {
        private readonly IIntegrationEventHandler<TMessage> _handler;
        private readonly NServiceBusMessageBus _messageBus;

        public NServiceBusMessageHandler(IIntegrationEventHandler<TMessage> handler, NServiceBusMessageBus messageBus)
        {
            _handler = handler;
            _messageBus = messageBus;
        }

        public async Task Handle(TMessage message, IMessageHandlerContext context)
        {
            _messageBus.ActiveContext = context;

            await _handler.HandleAsync(message, context.CancellationToken);
        }
    }
}