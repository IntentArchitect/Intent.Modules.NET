using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Associations
{
    public class O_DestNameDiff
    {
        public Guid Id { get; set; }

        public virtual ICollection<O_DestNameDiffDependent> DestNameDiff { get; set; } = [];
    }
}