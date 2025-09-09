using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested
{
    public class Two
    {
        public Two()
        {
            TwoName = null!;
        }

        public Guid Id { get; set; }

        public string TwoName { get; set; }

        public int TwoAge { get; set; }
    }
}