using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests
{
    public class NestingParent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<NestingChild> NestingChildren { get; set; } = new List<NestingChild>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}