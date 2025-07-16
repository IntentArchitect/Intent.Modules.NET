using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TableStorage.Tests.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Invoice()
        {
            OrderPartitionKey = null!;
            OrderRowKey = null!;
            Order = null!;
        }

        public string PartitionKey { get; set; } = Guid.NewGuid().ToString();

        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        public DateTime IssuedData { get; set; }

        public string OrderPartitionKey { get; set; }

        public string OrderRowKey { get; set; }

        public Order Order { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}