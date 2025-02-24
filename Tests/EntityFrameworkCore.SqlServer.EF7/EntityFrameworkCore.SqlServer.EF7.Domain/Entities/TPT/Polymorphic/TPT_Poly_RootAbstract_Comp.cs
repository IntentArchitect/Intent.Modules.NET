using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_RootAbstract_Comp
    {
        public TPT_Poly_RootAbstract_Comp()
        {
            CompField = null!;
        }
        public Guid Id { get; set; }

        public string CompField { get; set; }
    }
}