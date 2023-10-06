using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompositeSingleA
    {
        public CompositeSingleA()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public virtual CompositeSingleAA? Composite { get; set; }

        public virtual ICollection<CompositeManyAA> Composites { get; set; } = new List<CompositeManyAA>();
    }
}