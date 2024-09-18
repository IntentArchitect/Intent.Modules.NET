using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot2Composition : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual AggregateRoot2Single AggregateRoot2Single { get; set; }

        public virtual AggregateRoot2Nullable? AggregateRoot2Nullable { get; set; }

        public virtual ICollection<AggregateRoot2Collection> AggregateRoot2Collections { get; set; } = new List<AggregateRoot2Collection>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}