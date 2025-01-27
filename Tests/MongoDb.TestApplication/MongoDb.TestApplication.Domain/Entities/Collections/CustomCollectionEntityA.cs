using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Collections
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CustomCollectionEntityA
    {
        public CustomCollectionEntityA()
        {
            Id = null!;
            Attribute = null!;
        }
        public string Id { get; set; }

        public string Attribute { get; set; }
    }
}