using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping
{
    public class MappingCompositeMultiple
    {
        public Guid Id { get; private set; }

        public string MultipleValue { get; private set; }

        public Guid MappingRootId { get; private set; }
    }
}