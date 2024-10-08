using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Indexes
{
    public class CustomIndex
    {
        public Guid Id { get; set; }

        public Guid IndexField { get; set; }
    }
}