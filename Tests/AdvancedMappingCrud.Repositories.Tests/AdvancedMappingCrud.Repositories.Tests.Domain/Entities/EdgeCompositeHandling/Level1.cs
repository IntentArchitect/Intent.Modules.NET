using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling
{
    public class Level1
    {
        public Guid Id { get; set; }

        public Guid RootId { get; set; }

        public virtual ICollection<Level2> Level2s { get; set; } = [];
    }
}