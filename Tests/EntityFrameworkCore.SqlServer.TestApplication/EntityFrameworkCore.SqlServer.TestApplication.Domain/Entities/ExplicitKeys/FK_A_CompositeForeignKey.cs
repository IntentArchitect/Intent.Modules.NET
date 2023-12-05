using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class FK_A_CompositeForeignKey : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual PK_A_CompositeKey PK_A_CompositeKey { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public Guid PK_A_CompositeKeyCompositeKeyA { get; set; }

        public Guid PK_A_CompositeKeyCompositeKeyB { get; set; }
    }
}