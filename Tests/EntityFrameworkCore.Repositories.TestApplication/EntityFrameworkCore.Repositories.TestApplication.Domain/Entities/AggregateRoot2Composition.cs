using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateRoot2Composition : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual AggregateRoot2Single AggregateRoot2Single { get; set; }

        public virtual AggregateRoot2Nullable? AggregateRoot2Nullable { get; set; }

        public virtual ICollection<AggregateRoot2Collection> AggregateRoot2Collections { get; set; } = new List<AggregateRoot2Collection>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}