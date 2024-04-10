using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes
{
    public class SingleChildIndex
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}