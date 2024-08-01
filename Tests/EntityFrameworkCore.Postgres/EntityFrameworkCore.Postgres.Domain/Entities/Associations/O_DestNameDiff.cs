using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class O_DestNameDiff
    {
        public Guid Id { get; set; }

        public virtual ICollection<O_DestNameDiffDependent> DestNameDiff { get; set; } = new List<O_DestNameDiffDependent>();
    }
}