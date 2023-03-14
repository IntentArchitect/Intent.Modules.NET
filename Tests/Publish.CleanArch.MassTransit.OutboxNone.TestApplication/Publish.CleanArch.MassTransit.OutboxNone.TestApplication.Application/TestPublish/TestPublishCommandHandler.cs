using System;
using System.Threading;
using System.Threading.Tasks;
using Eventing;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.TestPublish
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestPublishCommandHandler : IRequestHandler<TestPublishCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
        public TestPublishCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(TestPublishCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new EventStartedEvent() { Message = request.Message });
            return Unit.Value;
        }
    }
}