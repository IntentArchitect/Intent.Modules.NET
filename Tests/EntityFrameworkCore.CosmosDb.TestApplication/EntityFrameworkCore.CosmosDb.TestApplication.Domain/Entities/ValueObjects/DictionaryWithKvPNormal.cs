using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class DictionaryWithKvPNormal : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairNormal> KeyValuePairNormals { get; set; } = new List<KeyValuePairNormal>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}