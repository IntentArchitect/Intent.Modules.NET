using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested
{
    public class Five
    {
        public Five()
        {
            FiveName = null!;
        }

        public Guid Id { get; set; }

        public string FiveName { get; set; }

        public Guid OneId { get; set; }

        public int FiveAge { get; set; }
    }
}