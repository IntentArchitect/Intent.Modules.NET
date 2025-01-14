using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class CheckNewCompChildCrud : IHasDomainEvent
    {
        public CheckNewCompChildCrud()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CNCCChild> CNCCChildren { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}