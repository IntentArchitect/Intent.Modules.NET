using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Indexes
{
    public class SortDirectionIndex
    {
        public Guid Id { get; set; }

        public string FieldA { get; set; }

        public string FieldB { get; set; }
    }
}