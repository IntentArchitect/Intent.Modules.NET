using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CosmosDB.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Region : IHasDomainEvent
    {
        private string? _id;
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<Country> Countries { get; set; } = new List<Country>();

        public virtual List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}