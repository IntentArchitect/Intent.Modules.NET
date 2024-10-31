using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities
{
    public class Second : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public int OneId { get; set; }

        public int Twoid { get; set; }

        public virtual ICollection<Three> Threes { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}