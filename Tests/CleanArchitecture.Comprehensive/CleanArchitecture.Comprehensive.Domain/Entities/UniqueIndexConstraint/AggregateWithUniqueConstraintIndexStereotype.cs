using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexStereotype : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SingleUniqueField { get; set; }

        public string CompUniqueFieldA { get; set; }

        public string CompUniqueFieldB { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}