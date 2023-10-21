using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class K_MultipleDependent
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public ICollection<string> JMultipleaggregatesIds { get; set; } = new List<string>();
    }
}