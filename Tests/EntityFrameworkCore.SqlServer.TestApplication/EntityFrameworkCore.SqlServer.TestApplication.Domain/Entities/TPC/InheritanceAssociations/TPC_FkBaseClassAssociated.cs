using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPC_FkBaseClassAssociated : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkBaseClassCompositeKeyA { get; set; }

        public Guid FkBaseClassCompositeKeyB { get; set; }

        public virtual TPC_FkBaseClass FkBaseClass { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}