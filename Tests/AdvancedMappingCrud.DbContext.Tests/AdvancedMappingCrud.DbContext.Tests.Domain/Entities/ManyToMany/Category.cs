using System;
using System.Collections.Generic;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Domain.Entities.ManyToMany
{
    public class Category : IHasDomainEvent
    {
        public Category()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}