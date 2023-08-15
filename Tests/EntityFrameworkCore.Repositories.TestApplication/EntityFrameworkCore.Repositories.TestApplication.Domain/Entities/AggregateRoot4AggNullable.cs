using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateRoot4AggNullable : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid AggregateRoot4SingleId { get; set; }

        public virtual AggregateRoot4Single AggregateRoot4Single { get; set; }

        public virtual ICollection<AggregateRoot4Collection> AggregateRoot4Collections { get; set; } = new List<AggregateRoot4Collection>();

        public Guid? AggregateRoot4NullableId { get; set; }

        public virtual AggregateRoot4Nullable? AggregateRoot4Nullable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}