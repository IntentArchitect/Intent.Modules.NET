using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPH_MiddleAbstract_Root : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public TPH_MiddleAbstract_Root()
        {
            RootAttribute = null!;
        }

        public Guid Id { get; set; }

        public string RootAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}