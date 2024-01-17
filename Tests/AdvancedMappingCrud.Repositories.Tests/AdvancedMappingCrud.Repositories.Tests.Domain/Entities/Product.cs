using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}