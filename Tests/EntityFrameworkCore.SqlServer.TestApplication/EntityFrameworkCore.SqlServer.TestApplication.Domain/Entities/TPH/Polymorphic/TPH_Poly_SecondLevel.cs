using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPH_Poly_SecondLevel : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<TPH_Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = new List<TPH_Poly_BaseClassNonAbstract>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}