using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public virtual Preferences? Preferences { get; set; }

        public virtual ICollection<Address> Address { get; set; } = new List<Address>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}