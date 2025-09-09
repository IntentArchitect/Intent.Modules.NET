using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested
{
    public class Four
    {
        public Four()
        {
            FourName = null!;
        }

        public Guid Id { get; set; }

        public string FourName { get; set; }

        public int FourAge { get; set; }

        public virtual Three? Three { get; set; }
    }
}