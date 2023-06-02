using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Client : IHasDomainEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> TagsIds { get; set; } = new List<string>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}