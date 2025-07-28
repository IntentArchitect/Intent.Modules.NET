using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling
{
    public class Level2
    {
        public Guid Id { get; set; }

        public Guid Level1Id { get; set; }
    }
}