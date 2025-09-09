using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested
{
    public class Three
    {
        public Three()
        {
            ThreeName = null!;
        }

        public Guid Id { get; set; }

        public string ThreeName { get; set; }

        public int ThreeAge { get; set; }
    }
}