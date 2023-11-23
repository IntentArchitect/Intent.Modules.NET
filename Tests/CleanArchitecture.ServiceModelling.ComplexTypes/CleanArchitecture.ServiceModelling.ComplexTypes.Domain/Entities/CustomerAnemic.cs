using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities
{
    public class CustomerAnemic : IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}