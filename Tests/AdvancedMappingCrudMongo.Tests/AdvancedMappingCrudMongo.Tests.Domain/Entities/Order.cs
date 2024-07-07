using System;
using System.Collections.Generic;
using AdvancedMappingCrudMongo.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public string RefNo { get; set; }

        public DateTime OrderDate { get; set; }

        public string ExternalRef { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}