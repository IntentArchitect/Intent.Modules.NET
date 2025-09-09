using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_RootAbstract_Aggr
    {
        public TPC_Poly_RootAbstract_Aggr()
        {
            AggrField = null!;
        }

        public Guid Id { get; set; }

        public string AggrField { get; set; }
    }
}