using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities
{
    public class Purchase : IHasDomainEvent
    {

        public Guid Id { get; set; }

        public Money Cost { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}