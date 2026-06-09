using Intent.RoslynWeaver.Attributes;
using MediatR;
using NServiceBus.RabbitMQ.Application.Common.Eventing;
using NServiceBus.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace NServiceBus.RabbitMQ.Application.ExternalMessagePublish.PublishExternalMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PublishExternalMessageCommandHandler : IRequestHandler<PublishExternalMessageCommand>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public PublishExternalMessageCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PublishExternalMessageCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Publish(new TestMessageEvent
            {
            });
        }
    }
}