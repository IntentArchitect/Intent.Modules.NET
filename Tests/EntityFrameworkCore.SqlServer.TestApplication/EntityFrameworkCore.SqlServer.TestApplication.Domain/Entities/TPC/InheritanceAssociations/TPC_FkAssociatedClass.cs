using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPC_FkAssociatedClass : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public TPC_FkAssociatedClass()
        {
            AssociatedField = null!;
            FkDerivedClass = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkDerivedClassCompositeKeyA { get; set; }

        public Guid FkDerivedClassCompositeKeyB { get; set; }

        public virtual TPC_FkDerivedClass FkDerivedClass { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}