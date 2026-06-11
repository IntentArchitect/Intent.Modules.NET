using Intent.RoslynWeaver.Attributes;
using MediatR;
using NServiceBus.OutboxPattern.Publish.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Publish.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace NServiceBus.OutboxPattern.Publish.Application.TestEventSend
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestEventSendCommandHandler : IRequestHandler<TestEventSendCommand>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public TestEventSendCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(TestEventSendCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Publish(new TestEvent
            {
                Message = request.Message
            });
        }
    }
}