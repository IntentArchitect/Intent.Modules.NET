using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
    public class Transaction : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Money Current { get; set; }

        public string Description { get; set; }

        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}