using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class P_SourceNameDiff
    {
        public Guid Id { get; set; }

        public virtual ICollection<P_SourceNameDiffDependent> P_SourceNameDiffDependents { get; set; } = new List<P_SourceNameDiffDependent>();
    }
}