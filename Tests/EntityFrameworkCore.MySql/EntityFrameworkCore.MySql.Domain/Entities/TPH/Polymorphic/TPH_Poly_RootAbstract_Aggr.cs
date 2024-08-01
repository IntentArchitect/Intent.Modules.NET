using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPH.Polymorphic
{
    public class TPH_Poly_RootAbstract_Aggr
    {
        public Guid Id { get; set; }

        public string AggrField { get; set; }
    }
}