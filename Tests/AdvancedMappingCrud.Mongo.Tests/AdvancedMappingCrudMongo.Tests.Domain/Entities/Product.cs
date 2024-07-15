using System;
using System.Collections.Generic;
using AdvancedMappingCrudMongo.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}