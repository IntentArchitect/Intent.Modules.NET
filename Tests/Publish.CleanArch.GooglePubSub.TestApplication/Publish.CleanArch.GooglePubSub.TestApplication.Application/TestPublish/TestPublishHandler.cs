using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.GooglePubSub.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Application.TestPublish
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestPublishHandler : IRequestHandler<TestPublish>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
        public TestPublishHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestPublish request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new EventStartedEvent() { Message = request.Message });

        }
    }
}