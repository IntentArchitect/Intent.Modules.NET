using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Indexes
{
    public class MultiChildIndex
    {
        public MultiChildIndex()
        {
            Name = null!;
            Surname = null!;
        }
        public Guid Id { get; set; }

        public Guid ParentIndexId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}