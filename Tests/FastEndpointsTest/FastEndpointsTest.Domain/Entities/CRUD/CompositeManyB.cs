using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class CompositeManyB
    {
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public Guid AggregateRootId { get; set; }

        public virtual CompositeSingleBB? Composite { get; set; }

        public virtual ICollection<CompositeManyBB> Composites { get; set; } = [];
    }
}