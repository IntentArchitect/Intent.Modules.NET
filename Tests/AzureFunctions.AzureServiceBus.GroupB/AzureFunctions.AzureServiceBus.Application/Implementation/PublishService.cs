using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Application.Interfaces;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PublishService : IPublishService
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public PublishService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task PublishEvent(PayloadDto payload, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new PublishAndConsumeMessageEvent
            {
                Data = payload.Data
            });
        }
    }
}