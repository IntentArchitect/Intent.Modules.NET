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
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class P_SourceNameDiff : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual ICollection<P_SourceNameDiffDependent> P_SourceNameDiffDependents { get; set; } = new List<P_SourceNameDiffDependent>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}