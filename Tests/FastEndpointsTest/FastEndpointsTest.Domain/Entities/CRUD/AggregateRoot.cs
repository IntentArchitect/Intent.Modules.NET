using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class AggregateRoot : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public string LimitedDomain { get; set; }

        public string LimitedService { get; set; }

        public EnumWithoutValues EnumType1 { get; set; }

        public EnumWithDefaultLiteral EnumType2 { get; set; }

        public EnumWithoutDefaultLiteral EnumType3 { get; set; }

        public Guid? AggregateId { get; set; }

        public virtual ICollection<CompositeManyB> Composites { get; set; } = new List<CompositeManyB>();

        public virtual CompositeSingleA? Composite { get; set; }

        public virtual AggregateSingleC? Aggregate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}