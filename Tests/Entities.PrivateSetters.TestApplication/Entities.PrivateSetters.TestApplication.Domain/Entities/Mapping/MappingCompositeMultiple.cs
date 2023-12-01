using System;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping
{
    public class MappingCompositeMultiple
    {
        public Guid Id { get; private set; }

        public string MultipleValue { get; private set; }

        public Guid MappingRootId { get; private set; }
    }
}