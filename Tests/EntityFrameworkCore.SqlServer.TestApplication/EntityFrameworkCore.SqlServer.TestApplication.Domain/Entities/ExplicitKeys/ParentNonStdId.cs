using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys
{
    public class ParentNonStdId : IHasDomainEvent
    {
        public Guid MyId { get; set; }

        public string Desc { get; set; }

        public virtual ChildNonStdId ChildNonStdId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}