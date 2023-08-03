using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Privilege : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public Privilege()
        {
            Id = null!;
            Name = null!;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}