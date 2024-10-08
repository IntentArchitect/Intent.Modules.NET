using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Indexes
{
    public class WithBaseIndexBase
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }
    }
}