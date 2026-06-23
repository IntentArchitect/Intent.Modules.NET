using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.AzureFunctions.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}