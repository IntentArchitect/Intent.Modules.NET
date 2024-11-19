using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.Polymorphic
{
    public class TPH_Poly_RootAbstract_Comp
    {
        public TPH_Poly_RootAbstract_Comp()
        {
            CompField = null!;
        }
        public Guid Id { get; set; }

        public string CompField { get; set; }
    }
}