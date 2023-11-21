using System;
using System.Collections.Generic;
using CleanArchitecture.SingleFiles.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class EfInvoice : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<EfLine> EfLines { get; set; } = new List<EfLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}