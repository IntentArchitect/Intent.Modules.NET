using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }

        public virtual Product Product { get; set; }
    }
}