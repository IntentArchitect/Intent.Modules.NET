using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using Intent.RoslynWeaver.Attributes;

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