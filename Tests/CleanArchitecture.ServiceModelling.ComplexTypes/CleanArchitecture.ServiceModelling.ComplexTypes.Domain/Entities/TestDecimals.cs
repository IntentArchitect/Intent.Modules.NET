using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities
{
    public class TestDecimals : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public decimal Price1 { get; set; }

        public decimal Price2 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}