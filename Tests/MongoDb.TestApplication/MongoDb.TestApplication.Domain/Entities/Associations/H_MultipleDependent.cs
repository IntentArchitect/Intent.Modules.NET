using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class H_MultipleDependent
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public string? HOptionalaggregatenavId { get; set; }
    }
}