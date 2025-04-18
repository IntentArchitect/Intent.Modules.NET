using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Indexes
{
    public class ParentIndex
    {
        public ParentIndex()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<MultiChildIndex> MultiChildIndices { get; set; } = new List<MultiChildIndex>();

        public virtual SingleChildIndex? SingleChildIndex { get; set; }
    }
}