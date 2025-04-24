using System;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities
{
    public class TemporalProduct : ITemporal
    {
        public TemporalProduct()
        {
            Name = null!;
        }

        public Guid Id1 { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}