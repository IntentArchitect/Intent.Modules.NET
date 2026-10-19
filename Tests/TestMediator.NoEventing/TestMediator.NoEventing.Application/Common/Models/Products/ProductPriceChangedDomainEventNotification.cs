using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.TypedDomainEventNotification", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Common.Models.Products
{
    public sealed class ProductPriceChangedDomainEventNotification : DomainEventNotification<ProductPriceChangedDomainEvent>
    {
        public ProductPriceChangedDomainEventNotification(ProductPriceChangedDomainEvent domainEvent) : base(domainEvent)
        {
        }
    }
}