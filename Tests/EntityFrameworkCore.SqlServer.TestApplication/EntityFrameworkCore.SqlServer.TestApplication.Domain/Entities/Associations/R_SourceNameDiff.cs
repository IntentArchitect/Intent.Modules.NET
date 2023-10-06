using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class R_SourceNameDiff : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual R_SourceNameDiffDependent R_SourceNameDiffDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}