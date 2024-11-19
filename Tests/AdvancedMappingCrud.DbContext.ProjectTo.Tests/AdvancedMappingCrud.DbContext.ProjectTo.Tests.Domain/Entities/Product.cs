using System;
using System.Collections.Generic;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities
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

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}