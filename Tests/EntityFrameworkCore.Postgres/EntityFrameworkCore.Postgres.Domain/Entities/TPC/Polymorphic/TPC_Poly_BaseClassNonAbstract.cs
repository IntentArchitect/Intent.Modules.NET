using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_BaseClassNonAbstract : TPC_Poly_RootAbstract
    {
        public TPC_Poly_BaseClassNonAbstract()
        {
            BaseField = null!;
        }
        public Guid Id { get; set; }

        public string BaseField { get; set; }

        public Guid? Poly_SecondLevelId { get; set; }
    }
}