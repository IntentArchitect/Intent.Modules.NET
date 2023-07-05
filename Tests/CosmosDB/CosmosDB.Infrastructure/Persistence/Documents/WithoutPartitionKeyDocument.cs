using System;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class WithoutPartitionKeyDocument : WithoutPartitionKey, ICosmosDBDocument<WithoutPartitionKeyDocument, WithoutPartitionKey>
    {
        private string? _type;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Id;
            set => Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }

        public WithoutPartitionKeyDocument PopulateFromEntity(WithoutPartitionKey entity)
        {
            Id = entity.Id;

            return this;
        }
    }
}