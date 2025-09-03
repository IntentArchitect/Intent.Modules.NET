using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPC.Polymorphic
{
    public abstract class TPC_Poly_RootAbstract
    {
        public TPC_Poly_RootAbstract()
        {
            AbstractField = null!;
        }

        public string AbstractField { get; set; }

        public Guid? Poly_RootAbstract_AggrId { get; set; }

        public virtual TPC_Poly_RootAbstract_Aggr? Poly_RootAbstract_Aggr { get; set; }

        public virtual TPC_Poly_RootAbstract_Comp? Poly_RootAbstract_Comp { get; set; }
    }
}