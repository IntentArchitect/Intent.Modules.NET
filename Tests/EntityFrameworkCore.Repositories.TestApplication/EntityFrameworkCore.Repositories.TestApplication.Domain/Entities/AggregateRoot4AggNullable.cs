using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot4AggNullable : IHasDomainEvent
    {
        public AggregateRoot4AggNullable()
        {
            AggregateRoot4Single = null!;
        }
        public Guid Id { get; set; }

        public Guid AggregateRoot4SingleId { get; set; }

        public virtual AggregateRoot4Single AggregateRoot4Single { get; set; }

        public virtual ICollection<AggregateRoot4Collection> AggregateRoot4Collections { get; set; } = [];

        public Guid? AggregateRoot4NullableId { get; set; }

        public virtual AggregateRoot4Nullable? AggregateRoot4Nullable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}