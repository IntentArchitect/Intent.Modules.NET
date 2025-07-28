using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling
{
    public class External : IHasDomainEvent
    {
        public External()
        {
            Level2 = null!;
        }

        public Guid Id { get; set; }

        public Guid Level2Id { get; set; }

        public virtual Level2 Level2 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}