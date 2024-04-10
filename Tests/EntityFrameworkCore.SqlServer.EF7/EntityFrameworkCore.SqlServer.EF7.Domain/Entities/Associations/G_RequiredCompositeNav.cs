using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class G_RequiredCompositeNav
    {
        public Guid Id { get; set; }

        public string ReqCompNavAttr { get; set; }

        public virtual ICollection<G_MultipleDependent> G_MultipleDependents { get; set; } = new List<G_MultipleDependent>();
    }
}