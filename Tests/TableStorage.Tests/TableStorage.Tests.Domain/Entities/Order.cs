using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Common;

namespace TableStorage.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string OrderNo { get; set; }

        public decimal Amount { get; set; }

        public Customer Customer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}