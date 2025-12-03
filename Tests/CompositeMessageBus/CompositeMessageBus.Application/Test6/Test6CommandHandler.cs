using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CompositeMessageBus.Application.Test6
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Test6CommandHandler : IRequestHandler<Test6Command>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public Test6CommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Test6Command request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new MsgMassTEvent
            {
                Message = request.Message
            });
        }
    }
}