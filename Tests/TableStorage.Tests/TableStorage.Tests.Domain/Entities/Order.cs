using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TableStorage.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string OrderNo { get; set; }

        public decimal Amount { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}