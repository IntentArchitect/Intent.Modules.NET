using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling
{
    public class Root : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public virtual ICollection<Level1> Level1s { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}