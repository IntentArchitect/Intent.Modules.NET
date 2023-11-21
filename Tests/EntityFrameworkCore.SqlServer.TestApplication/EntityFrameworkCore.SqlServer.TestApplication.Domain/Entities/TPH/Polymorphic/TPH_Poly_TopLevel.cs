using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPH_Poly_TopLevel : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string TopField { get; set; }

        public virtual ICollection<TPH_Poly_RootAbstract> RootAbstracts { get; set; } = new List<TPH_Poly_RootAbstract>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}