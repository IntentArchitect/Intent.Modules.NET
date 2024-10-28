using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class AggregateRootLong : IHasDomainEvent
    {
        public long Id { get; set; }

        public string Attribute { get; set; }

        public virtual CompositeOfAggrLong? CompositeOfAggrLong { get; set; } = new();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}