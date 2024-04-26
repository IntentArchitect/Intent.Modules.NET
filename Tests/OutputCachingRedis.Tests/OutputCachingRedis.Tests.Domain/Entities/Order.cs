using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace OutputCachingRedis.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}