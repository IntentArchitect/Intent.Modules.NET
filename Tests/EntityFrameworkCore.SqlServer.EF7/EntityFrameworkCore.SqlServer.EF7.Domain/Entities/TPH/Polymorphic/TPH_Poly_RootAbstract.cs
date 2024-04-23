using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.Polymorphic
{
    public abstract class TPH_Poly_RootAbstract
    {
        public Guid Id { get; set; }

        public string AbstractField { get; set; }

        public Guid? Poly_TopLevelId { get; set; }

        public Guid? Poly_RootAbstract_AggrId { get; set; }

        public virtual TPH_Poly_RootAbstract_Aggr? Poly_RootAbstract_Aggr { get; set; }

        public virtual TPH_Poly_RootAbstract_Comp? Poly_RootAbstract_Comp { get; set; }
    }
}