using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_RootAbstract_Aggr
    {
        public TPT_Poly_RootAbstract_Aggr()
        {
            AggrField = null!;
        }
        public Guid Id { get; set; }

        public string AggrField { get; set; }
    }
}