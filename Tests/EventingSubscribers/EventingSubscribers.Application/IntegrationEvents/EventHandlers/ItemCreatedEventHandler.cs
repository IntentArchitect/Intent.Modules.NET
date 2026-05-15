using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Entities;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ItemCreatedEventHandler : IIntegrationEventHandler<ItemCreatedEvent>
    {
        private readonly IItemRepository _itemRepository;

        [IntentManaged(Mode.Merge)]
        public ItemCreatedEventHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ItemCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var createItem = new Item
            {
                Category = message.Category
            };

            _itemRepository.Add(createItem);
        }
    }
}