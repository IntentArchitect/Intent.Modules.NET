using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunction.QueueStorage.Application.Common.Eventing;
using AzureFunction.QueueStorage.Eventing.Messages;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunction.QueueStorage.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateProductCommandHandler : IIntegrationEventHandler<CreateProductCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(CreateProductCommand message, CancellationToken cancellationToken = default)
        {
            _eventBus.Publish(new ProductCreatedEvent
            {
                Name = message.Name,
                Qty = message.Qty
            });
        }
    }
}