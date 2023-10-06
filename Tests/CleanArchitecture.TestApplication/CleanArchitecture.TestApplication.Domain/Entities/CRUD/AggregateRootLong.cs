using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateRootLong : IHasDomainEvent
    {
        public AggregateRootLong()
        {
            Attribute = null!;
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

        public virtual CompositeOfAggrLong? CompositeOfAggrLong { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}