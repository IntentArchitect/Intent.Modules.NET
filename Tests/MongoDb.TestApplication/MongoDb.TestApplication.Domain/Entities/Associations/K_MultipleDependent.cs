using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class K_MultipleDependent
    {
        [IntentManaged(Mode.Fully)]
        public K_MultipleDependent()
        {
            Id = null!;
            Attribute = null!;
        }
        public string Id { get; set; }

        public string Attribute { get; set; }

        public ICollection<string> JMultipleaggregatesIds { get; set; } = new List<string>();
    }
}