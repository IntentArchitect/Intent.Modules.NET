using System;
using System.Collections.Generic;
using CleanArchitecture.SingleFiles.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class EfInvoice : IHasDomainEvent
    {
        public EfInvoice()
        {
            Description = null!;
        }
        public Guid Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<EfLine> EfLines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}