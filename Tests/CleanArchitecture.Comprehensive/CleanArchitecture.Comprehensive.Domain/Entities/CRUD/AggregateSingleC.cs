using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    public class AggregateSingleC : IHasDomainEvent
    {
        public AggregateSingleC()
        {
            AggregationAttr = null!;
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}