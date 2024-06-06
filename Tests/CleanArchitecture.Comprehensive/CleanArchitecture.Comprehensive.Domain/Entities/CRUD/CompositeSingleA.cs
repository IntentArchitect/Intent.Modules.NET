using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompositeSingleA
    {

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public virtual CompositeSingleAA? Composite { get; set; }

        public virtual ICollection<CompositeManyAA> Composites { get; set; } = new List<CompositeManyAA>();
    }
}