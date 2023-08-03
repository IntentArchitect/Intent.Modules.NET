using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Inhabitant : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public Inhabitant()
        {
            InhabitantAttribute = null!;
        }
        public Guid Id { get; set; }

        public string InhabitantAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        protected virtual ICollection<Branch> Branches { get; set; }
    }
}