using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.Associations
{
    public class D_OptionalAggregate
    {
        public D_OptionalAggregate()
        {
            OptionalAggrAttr = null!;
        }
        public Guid Id { get; set; }

        public string OptionalAggrAttr { get; set; }

        public virtual ICollection<D_MultipleDependent> D_MultipleDependents { get; set; } = [];
    }
}