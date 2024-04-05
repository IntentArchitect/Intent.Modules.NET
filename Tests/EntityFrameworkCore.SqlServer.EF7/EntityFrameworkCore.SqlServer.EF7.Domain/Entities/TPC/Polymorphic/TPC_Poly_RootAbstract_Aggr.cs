using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_RootAbstract_Aggr
    {
        public Guid Id { get; set; }

        public string AggrField { get; set; }
    }
}