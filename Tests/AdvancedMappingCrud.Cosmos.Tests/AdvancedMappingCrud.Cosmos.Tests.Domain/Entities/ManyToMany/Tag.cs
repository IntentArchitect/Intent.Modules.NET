using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany
{
    public class Tag : IHasDomainEvent
    {
        private Guid? _id;

        public Tag()
        {
            Name = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Name { get; set; }

        public IList<Guid> ProductItemsIds { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}