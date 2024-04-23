using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class F_OptionalAggregateNav
    {
        public Guid Id { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public Guid? F_OptionalDependentId { get; set; }

        public virtual F_OptionalDependent? F_OptionalDependent { get; set; }
    }
}