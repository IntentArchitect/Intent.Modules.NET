using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CompositeMessageBus.Application.Test3
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Test3CommandHandler : IRequestHandler<Test3Command>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public Test3CommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Test3Command request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new MsgAzSrvBusEvent
            {
                Message = request.Message
            });
        }
    }
}