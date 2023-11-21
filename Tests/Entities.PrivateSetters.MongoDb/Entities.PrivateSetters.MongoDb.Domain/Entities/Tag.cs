using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Entities.PrivateSetters.MongoDb.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Tag
    {
        public Tag(string name)
        {
            Name = name;
        }
        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}