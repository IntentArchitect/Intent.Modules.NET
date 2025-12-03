using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CompositeMessageBus.Application.Test1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Test1CommandHandler : IRequestHandler<Test1Command>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public Test1CommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Test1Command request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new MsgAzEvtGridEvent
            {
                Message = request.Message
            });
        }
    }
}