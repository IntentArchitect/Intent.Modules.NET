using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class E_RequiredCompositeNav
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public E_RequiredDependent E_RequiredDependent { get; set; }
    }
}