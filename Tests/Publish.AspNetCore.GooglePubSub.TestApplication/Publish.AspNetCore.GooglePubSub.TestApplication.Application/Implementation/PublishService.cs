using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Interfaces;
using Publish.AspNetCore.GooglePubSub.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PublishService : IPublishService
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
        public PublishService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestPublish(string message, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new EventStartedEvent() { Message = message });
        }

        public void Dispose()
        {
        }
    }
}