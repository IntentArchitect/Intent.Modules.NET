using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Indexing
{
    public class FilteredIndex : IHasDomainEvent
    {
        public FilteredIndex()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}