using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping
{
    public class MappingCompositeSingle
    {
        public Guid Id { get; private set; }

        public string SingleValue { get; private set; }
    }
}