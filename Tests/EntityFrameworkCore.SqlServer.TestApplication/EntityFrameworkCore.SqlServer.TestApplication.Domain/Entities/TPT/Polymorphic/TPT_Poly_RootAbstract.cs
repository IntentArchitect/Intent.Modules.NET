using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract class TPT_Poly_RootAbstract : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string AbstractField { get; set; }

        public Guid? Poly_RootAbstract_AggrId { get; set; }

        public virtual TPT_Poly_RootAbstract_Aggr? Poly_RootAbstract_Aggr { get; set; }

        public virtual TPT_Poly_RootAbstract_Comp? Poly_RootAbstract_Comp { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public Guid? Poly_TopLevelId { get; set; }
    }
}