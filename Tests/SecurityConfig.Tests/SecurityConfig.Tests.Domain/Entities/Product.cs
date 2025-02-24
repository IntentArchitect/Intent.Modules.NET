using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SecurityConfig.Tests.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
            Surname = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}