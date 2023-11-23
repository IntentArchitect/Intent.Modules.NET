using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPT_Poly_SecondLevel : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<TPT_Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = new List<TPT_Poly_BaseClassNonAbstract>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}