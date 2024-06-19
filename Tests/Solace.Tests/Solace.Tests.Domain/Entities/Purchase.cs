using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Solace.Tests.Domain.Entities
{
    public class Purchase : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public virtual Account Account { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}