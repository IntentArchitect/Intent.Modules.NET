using System;
using System.Collections.Generic;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Domain.Entities.ManyToMany
{
    public class ProductItem : IHasDomainEvent
    {
        public ProductItem()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = [];

        public virtual ICollection<Tag> Tags { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}