using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EfCore.SecondLevelCaching.Domain.Entities
{
    public class Invoice
    {
        public Invoice()
        {
            Notes = null!;
        }
        public Guid Id { get; set; }

        public string Notes { get; set; }
    }
}