using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes
{
    public class DeviationIndex
    {
        public Guid Id { get; set; }

        public string AttributeName { get; set; }
    }
}