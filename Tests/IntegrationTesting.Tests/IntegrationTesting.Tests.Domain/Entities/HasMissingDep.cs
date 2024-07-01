using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class HasMissingDep : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid MissingDepId { get; set; }

        public virtual MissingDep MissingDep { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}