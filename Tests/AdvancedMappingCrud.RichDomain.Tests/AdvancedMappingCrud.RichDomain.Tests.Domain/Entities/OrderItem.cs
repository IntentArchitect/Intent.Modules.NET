using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class OrderItem
    {
        public OrderItem(Guid productId, int quantity, decimal amount)
        {
            ProductId = productId;
            Quantity = quantity;
            Amount = amount;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OrderItem()
        {
            Product = null!;
        }

        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public decimal Amount { get; private set; }

        public virtual Product Product { get; private set; }

        public void UpdateDetails(Guid productId, int quantity, decimal amount)
        {
            ProductId = productId;
            Quantity = quantity;
            Amount = amount;
        }
    }
}