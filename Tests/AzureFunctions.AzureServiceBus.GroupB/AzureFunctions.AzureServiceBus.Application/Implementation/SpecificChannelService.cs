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
    public class SpecificChannelService : ISpecificChannelService
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public SpecificChannelService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task SendSpecificQueueOne(PayloadDto dto, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new SpecificQueueOneMessageEvent
            {
                FieldA = dto.Data
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task SendSpecificQueueTwo(PayloadDto dto, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new SpecificQueueTwoMessageEvent
            {
                FieldB = dto.Data
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task SendSpecificTopicOne(PayloadDto dto, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new SpecificTopicOneMessageEvent
            {
                FieldA = dto.Data
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task SendSpecificTopicTwo(PayloadDto dto, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new SpecificTopicTwoMessageEvent
            {
                FieldB = dto.Data
            });
        }
    }
}