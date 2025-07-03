using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class Country : IHasDomainEvent
    {
        public Country()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<State> States { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}