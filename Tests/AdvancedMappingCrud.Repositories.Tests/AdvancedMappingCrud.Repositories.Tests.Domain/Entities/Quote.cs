using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Quote : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public Guid PersonId { get; set; }

        public string? PersonEmail { get; set; }

        public virtual ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}