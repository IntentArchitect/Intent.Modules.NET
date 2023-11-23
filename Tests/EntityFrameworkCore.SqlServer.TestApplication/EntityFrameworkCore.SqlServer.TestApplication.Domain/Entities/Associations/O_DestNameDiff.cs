using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class O_DestNameDiff : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual ICollection<O_DestNameDiffDependent> DestNameDiff { get; set; } = new List<O_DestNameDiffDependent>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}