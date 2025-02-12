using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.IdTypes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class IdTypeGuid
    {
        public IdTypeGuid()
        {
            Attribute = null!;
        }
        public Guid Id { get; set; }

        public string Attribute { get; set; }
    }
}