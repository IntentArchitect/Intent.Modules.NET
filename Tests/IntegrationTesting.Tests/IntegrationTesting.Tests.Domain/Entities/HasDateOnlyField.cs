using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class HasDateOnlyField : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public DateOnly MyDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}