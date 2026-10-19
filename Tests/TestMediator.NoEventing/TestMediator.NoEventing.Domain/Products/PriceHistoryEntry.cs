using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.NoEventing.Domain.Products
{
    public class PriceHistoryEntry : IHasDomainEvent
    {
        public PriceHistoryEntry(Guid productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            ChangedAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected PriceHistoryEntry()
        {
        }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public decimal OldPrice { get; set; }

        public decimal NewPrice { get; set; }

        public DateTimeOffset ChangedAt { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}
