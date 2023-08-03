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
    public class AggregateRoot2Composition : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public AggregateRoot2Composition()
        {
            AggregateRoot2Single = null!;
        }
        public Guid Id { get; set; }

        public virtual AggregateRoot2Single AggregateRoot2Single { get; set; }

        public virtual AggregateRoot2Nullable? AggregateRoot2Nullable { get; set; }

        public virtual ICollection<AggregateRoot2Collection> AggregateRoot2Collections { get; set; } = new List<AggregateRoot2Collection>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}