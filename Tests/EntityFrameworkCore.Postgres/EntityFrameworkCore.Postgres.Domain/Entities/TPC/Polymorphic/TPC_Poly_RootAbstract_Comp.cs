using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_RootAbstract_Comp
    {
        public Guid Id { get; set; }

        public string CompField { get; set; }
    }
}