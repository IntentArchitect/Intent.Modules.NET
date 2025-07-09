using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    public class AggregateRootLong : IHasDomainEvent
    {
        public AggregateRootLong()
        {
            Attribute = null!;
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

        public virtual CompositeOfAggrLong? CompositeOfAggrLong { get; set; } = new();

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}