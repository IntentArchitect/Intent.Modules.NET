using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.Persistence.Sql.Publish.Application.Common.Eventing;
using N_ServiceBus.Persistence.Sql.Publish.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace N_ServiceBus.Persistence.Sql.Publish.Application.TestCommandSend
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandSendCommandHandler : IRequestHandler<TestCommandSendCommand>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public TestCommandSendCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(TestCommandSendCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Send(new TestCommand
            {
                Message = request.Message
            });
        }
    }
}