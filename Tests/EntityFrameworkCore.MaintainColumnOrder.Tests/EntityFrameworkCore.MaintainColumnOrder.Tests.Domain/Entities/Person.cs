using System;
using System.Collections.Generic;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities
{
    public class Person : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}