using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping
{
    public class MappingCompositeSingle
    {
        public Guid Id { get; private set; }

        public string SingleValue { get; private set; }
    }
}