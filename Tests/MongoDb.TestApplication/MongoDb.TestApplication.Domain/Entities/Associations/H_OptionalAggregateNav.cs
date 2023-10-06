using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class H_OptionalAggregateNav
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public ICollection<string> HMultipledependentsIds { get; set; } = new List<string>();
    }
}