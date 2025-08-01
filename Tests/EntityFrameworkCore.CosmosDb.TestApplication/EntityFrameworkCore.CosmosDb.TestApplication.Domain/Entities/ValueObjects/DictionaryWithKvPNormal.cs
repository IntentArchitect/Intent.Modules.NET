using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects
{
    public class DictionaryWithKvPNormal : IHasDomainEvent
    {
        public DictionaryWithKvPNormal()
        {
            PartitionKey = null!;
            Title = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairNormal> KeyValuePairNormals { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}