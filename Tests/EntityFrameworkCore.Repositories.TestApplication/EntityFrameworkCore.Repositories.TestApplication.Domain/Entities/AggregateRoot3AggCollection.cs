using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot3AggCollection : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid AggregateRoot3SingleId { get; set; }

        public virtual AggregateRoot3Single AggregateRoot3Single { get; set; }

        public Guid? AggregateRoot3NullableId { get; set; }

        public virtual AggregateRoot3Nullable? AggregateRoot3Nullable { get; set; }

        public Guid AggregateRoot3CollectionId { get; set; }

        public virtual AggregateRoot3Collection AggregateRoot3Collection { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}