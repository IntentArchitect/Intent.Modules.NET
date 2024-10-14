using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class OrderItem
    {
        public OrderItem(Guid productId, int quantity, decimal amount)
        {
            ProductId = productId;
            Quantity = quantity;
            Amount = amount;
        }

        public void UpdateDetails(Guid productId, int quantity, decimal amount)
        {
            ProductId = productId;
            Quantity = quantity;
            Amount = amount;
        }
    }
}