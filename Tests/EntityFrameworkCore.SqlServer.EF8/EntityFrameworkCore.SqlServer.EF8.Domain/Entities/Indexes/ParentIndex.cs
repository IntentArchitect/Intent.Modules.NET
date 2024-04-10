using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes
{
    public class ParentIndex
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<MultiChildIndex> MultiChildIndices { get; set; } = new List<MultiChildIndex>();

        public virtual SingleChildIndex? SingleChildIndex { get; set; }
    }
}