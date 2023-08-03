using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateRoot : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public AggregateRoot()
        {
            AggregateAttr = null!;
            LimitedDomain = null!;
            LimitedService = null!;
        }
        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public string LimitedDomain { get; set; }

        public string LimitedService { get; set; }

        public EnumWithoutValues EnumType1 { get; set; }

        public EnumWithDefaultLiteral EnumType2 { get; set; }

        public EnumWithoutDefaultLiteral EnumType3 { get; set; }

        public virtual ICollection<CompositeManyB> Composites { get; set; } = new List<CompositeManyB>();

        public virtual CompositeSingleA? Composite { get; set; }

        public Guid? AggregateId { get; set; }

        public virtual AggregateSingleC? Aggregate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}