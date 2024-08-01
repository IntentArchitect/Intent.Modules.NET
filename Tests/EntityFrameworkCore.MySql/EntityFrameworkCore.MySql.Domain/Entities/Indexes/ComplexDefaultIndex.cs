using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Indexes
{
    public class ComplexDefaultIndex
    {
        public Guid Id { get; set; }

        public Guid FieldA { get; set; }

        public Guid FieldB { get; set; }
    }
}