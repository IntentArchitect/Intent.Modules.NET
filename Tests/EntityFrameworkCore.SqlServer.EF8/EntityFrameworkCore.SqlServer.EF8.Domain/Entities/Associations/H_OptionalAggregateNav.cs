using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class H_OptionalAggregateNav
    {
        public H_OptionalAggregateNav()
        {
            OptionalAggrNavAttr = null!;
        }
        public Guid Id { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual ICollection<H_MultipleDependent> H_MultipleDependents { get; set; } = [];
    }
}