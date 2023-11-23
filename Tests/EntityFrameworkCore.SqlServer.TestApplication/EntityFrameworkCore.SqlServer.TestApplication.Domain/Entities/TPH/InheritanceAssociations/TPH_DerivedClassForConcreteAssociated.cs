using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPH_DerivedClassForConcreteAssociated : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public virtual TPH_DerivedClassForConcrete DerivedClassForConcrete { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}