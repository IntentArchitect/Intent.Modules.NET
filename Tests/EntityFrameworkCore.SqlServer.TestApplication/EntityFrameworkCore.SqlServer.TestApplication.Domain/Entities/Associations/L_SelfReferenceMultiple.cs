using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class L_SelfReferenceMultiple : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SelfRefMultipleAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; } = new List<L_SelfReferenceMultiple>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}