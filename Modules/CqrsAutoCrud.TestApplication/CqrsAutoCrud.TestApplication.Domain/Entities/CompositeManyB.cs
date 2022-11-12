using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge)]
    [DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore, Targets = Targets.Methods, AccessModifiers = AccessModifiers.Public)]
    public partial class CompositeManyB
    {
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AAggregaterootId { get; set; }

        public DateTime? SomeDate { get; set; }

        public virtual CompositeSingleBB? Composite { get; set; }

        public virtual ICollection<CompositeManyBB> Composites { get; set; } = new List<CompositeManyBB>();

        public Guid A_AggregateRootId { get; set; }

    }
}