using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class N_ComplexRoot : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string ComplexAttr { get; set; }

        public virtual N_CompositeOne N_CompositeOne { get; set; }

        public virtual N_CompositeTwo N_CompositeTwo { get; set; }

        public virtual ICollection<N_CompositeMany> N_CompositeManies { get; set; } = new List<N_CompositeMany>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}