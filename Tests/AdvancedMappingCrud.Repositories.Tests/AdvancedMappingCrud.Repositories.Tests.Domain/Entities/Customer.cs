using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public bool IsActive { get; set; }

        public virtual Preferences? Preferences { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}