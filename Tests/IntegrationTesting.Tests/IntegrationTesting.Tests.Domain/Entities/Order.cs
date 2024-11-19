using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            RefNo = null!;
            Customer = null!;
        }
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string RefNo { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}