using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class A_RequiredComposite
    {
        public string Id { get; set; }

        public string ReqCompAttribute { get; set; }

        public A_OptionalDependent? A_OptionalDependent { get; set; }
    }
}