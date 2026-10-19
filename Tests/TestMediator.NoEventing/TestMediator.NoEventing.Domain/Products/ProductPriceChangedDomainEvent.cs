using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace TestMediator.NoEventing.Domain.Products
{
    public class ProductPriceChangedDomainEvent : DomainEvent
    {
        public ProductPriceChangedDomainEvent(Guid productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }

        public Guid ProductId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
    }
}