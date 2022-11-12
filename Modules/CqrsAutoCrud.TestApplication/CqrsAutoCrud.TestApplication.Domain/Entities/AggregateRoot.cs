using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge)]
    [DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore, Targets = Targets.Methods, AccessModifiers = AccessModifiers.Public)]
    public partial class AggregateRoot
    {
        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public virtual CompositeSingleA? Composite { get; set; }

        public virtual ICollection<CompositeManyB> Composites { get; set; } = new List<CompositeManyB>();

        public Guid? AggregateId { get; set; }

        public virtual AggregateSingleC? Aggregate { get; set; }

    }
}