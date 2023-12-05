using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class P_SourceNameDiff : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual ICollection<P_SourceNameDiffDependent> P_SourceNameDiffDependents { get; set; } = new List<P_SourceNameDiffDependent>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}