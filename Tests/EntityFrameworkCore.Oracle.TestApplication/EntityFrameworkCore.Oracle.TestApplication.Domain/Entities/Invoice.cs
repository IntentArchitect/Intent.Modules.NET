using System;
using System.Collections.Generic;
using EntityFrameworkCore.Oracle.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Invoice()
        {
            Number = null!;
        }
        public Guid Id { get; set; }

        public string Number { get; set; }

        public virtual ICollection<LineItem> LineItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}