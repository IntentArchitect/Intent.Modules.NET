using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities
{
    public class One : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public int OneId { get; set; }

        public virtual ICollection<Second> Seconds { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}