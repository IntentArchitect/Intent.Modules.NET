using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    public class CompositeSingleA
    {
        public CompositeSingleA()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public virtual CompositeSingleAA? Composite { get; set; }

        public virtual ICollection<CompositeManyAA> Composites { get; set; } = [];
    }
}