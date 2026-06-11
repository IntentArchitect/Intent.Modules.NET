using Intent.RoslynWeaver.Attributes;
using NServiceBus.LearnerTransport.Application.Common.Eventing;
using NServiceBus.LearnerTransport.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageHandler", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Infrastructure.Eventing
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

    internal sealed class NServiceBusTestMessageEventHandler : NServiceBusMessageHandler<TestMessageEvent>
    {
        public NServiceBusTestMessageEventHandler(IIntegrationEventHandler<TestMessageEvent> handler,
            NServiceBusMessageBus messageBus) : base(handler, messageBus)
        {
        }
    }

    internal sealed class NServiceBusOrderAnimalHandler : NServiceBusMessageHandler<OrderAnimal>
    {
        public NServiceBusOrderAnimalHandler(IIntegrationEventHandler<OrderAnimal> handler,
            NServiceBusMessageBus messageBus) : base(handler, messageBus)
        {
        }
    }

    internal sealed class NServiceBusMakeSoundCommandHandler : NServiceBusMessageHandler<MakeSoundCommand>
    {
        public NServiceBusMakeSoundCommandHandler(IIntegrationEventHandler<MakeSoundCommand> handler,
            NServiceBusMessageBus messageBus) : base(handler, messageBus)
        {
        }
    }

    internal sealed class NServiceBusTalkToPersonCommandHandler : NServiceBusMessageHandler<TalkToPersonCommand>
    {
        public NServiceBusTalkToPersonCommandHandler(IIntegrationEventHandler<TalkToPersonCommand> handler,
            NServiceBusMessageBus messageBus) : base(handler, messageBus)
        {
        }
    }

    internal sealed class NServiceBusCreatePersonIdentityHandler : NServiceBusMessageHandler<CreatePersonIdentity>
    {
        public NServiceBusCreatePersonIdentityHandler(IIntegrationEventHandler<CreatePersonIdentity> handler,
            NServiceBusMessageBus messageBus) : base(handler, messageBus)
        {
        }
    }
}