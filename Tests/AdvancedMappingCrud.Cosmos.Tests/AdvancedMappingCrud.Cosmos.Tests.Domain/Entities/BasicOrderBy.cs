using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class BasicOrderBy : IHasDomainEvent
    {
        private string? _id;
        public BasicOrderBy()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Surname { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}