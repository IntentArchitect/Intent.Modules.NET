using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Application.Common.Models.Products;
using TestMediator.NoEventing.Domain.Products;
using TestMediator.NoEventing.Domain.Repositories.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace TestMediator.NoEventing.Application.EventHandlers.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProductPriceChangedDomainEventHandler : INotificationHandler<ProductPriceChangedDomainEventNotification>
    {
        private readonly IPriceHistoryEntryRepository _priceHistoryEntryRepository;

        [IntentManaged(Mode.Merge)]
        public ProductPriceChangedDomainEventHandler(IPriceHistoryEntryRepository priceHistoryEntryRepository)
        {
            _priceHistoryEntryRepository = priceHistoryEntryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask Handle(
            ProductPriceChangedDomainEventNotification notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _priceHistoryEntryRepository.Add(new PriceHistoryEntry(
                productId: domainEvent.ProductId,
                oldPrice: domainEvent.OldPrice,
                newPrice: domainEvent.NewPrice));
        }
    }
}
