using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class N_ComplexRoot : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string ComplexAttr { get; set; }

        public virtual N_CompositeOne N_CompositeOne { get; set; }

        public virtual N_CompositeTwo N_CompositeTwo { get; set; }

        public virtual ICollection<N_CompositeMany> N_CompositeManies { get; set; } = new List<N_CompositeMany>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}