using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class E_RequiredDependent
    {
        public string Attribute { get; set; }

        public E_RequiredCompositeNav E_RequiredCompositeNav { get; set; }
    }
}