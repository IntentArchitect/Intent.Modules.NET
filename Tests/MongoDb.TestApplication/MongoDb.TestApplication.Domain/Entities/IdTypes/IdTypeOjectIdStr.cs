using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.IdTypes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class IdTypeOjectIdStr
    {
        public string Id { get; set; }

        public string Attribute { get; set; }
    }
}