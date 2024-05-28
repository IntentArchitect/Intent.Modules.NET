using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}