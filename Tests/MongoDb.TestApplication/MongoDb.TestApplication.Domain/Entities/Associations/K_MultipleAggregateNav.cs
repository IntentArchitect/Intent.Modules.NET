using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class K_MultipleAggregateNav
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public ICollection<string> JMultipledependentsIds { get; set; } = new List<string>();
    }
}