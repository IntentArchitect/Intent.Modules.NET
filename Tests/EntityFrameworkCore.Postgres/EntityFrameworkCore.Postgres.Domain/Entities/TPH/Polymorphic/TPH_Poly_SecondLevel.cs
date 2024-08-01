using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic
{
    public class TPH_Poly_SecondLevel
    {
        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<TPH_Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = new List<TPH_Poly_BaseClassNonAbstract>();
    }
}