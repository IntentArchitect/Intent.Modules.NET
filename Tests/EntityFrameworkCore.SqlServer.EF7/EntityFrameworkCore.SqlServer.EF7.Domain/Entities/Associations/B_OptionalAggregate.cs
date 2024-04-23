using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class B_OptionalAggregate
    {
        public Guid Id { get; set; }

        public string OptionalAggrAttr { get; set; }

        public Guid? B_OptionalDependentId { get; set; }

        public virtual B_OptionalDependent? B_OptionalDependent { get; set; }
    }
}