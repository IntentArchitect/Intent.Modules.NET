using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class L_SelfReferenceMultiple
    {
        public Guid Id { get; set; }

        public string SelfRefMultipleAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; } = new List<L_SelfReferenceMultiple>();
    }
}