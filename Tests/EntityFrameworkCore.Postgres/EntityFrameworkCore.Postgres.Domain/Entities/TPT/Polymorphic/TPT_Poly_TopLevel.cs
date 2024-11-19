using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_TopLevel
    {
        public TPT_Poly_TopLevel()
        {
            TopField = null!;
        }
        public Guid Id { get; set; }

        public string TopField { get; set; }

        public virtual ICollection<TPT_Poly_RootAbstract> RootAbstracts { get; set; } = [];
    }
}