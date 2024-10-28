using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class AggregateSingleC : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}