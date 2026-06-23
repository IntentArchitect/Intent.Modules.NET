using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.Persistence.NHibernate.Publish.Application.Common.Eventing;
using N_ServiceBus.Persistence.NHibernate.Publish.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Application.TestEventSend
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