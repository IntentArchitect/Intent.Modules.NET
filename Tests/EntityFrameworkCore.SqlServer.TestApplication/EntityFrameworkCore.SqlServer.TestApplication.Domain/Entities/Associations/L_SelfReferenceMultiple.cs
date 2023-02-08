using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class L_SelfReferenceMultiple : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SelfRefMultipleAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; } = new List<L_SelfReferenceMultiple>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        protected virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesSrc { get; set; }
    }
}