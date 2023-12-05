using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Client : IHasDomainEvent
    {
        private string? _id;
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<string> TagsIds { get; set; } = new List<string>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}