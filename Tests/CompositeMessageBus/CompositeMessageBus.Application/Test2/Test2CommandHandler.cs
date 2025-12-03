using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CompositeMessageBus.Application.Test2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Test2CommandHandler : IRequestHandler<Test2Command>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public Test2CommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Test2Command request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new MsgQStorageEvent
            {
                Message = request.Message
            });
        }
    }
}