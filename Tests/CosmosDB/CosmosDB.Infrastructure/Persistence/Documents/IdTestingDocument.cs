using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class IdTestingDocument : IdTesting, ICosmosDBDocument<IdTestingDocument, IdTesting>
    {
        private string? _type;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Identifier;
            set => Identifier = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        [JsonProperty("@id")]
        public new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }
        [JsonIgnore]
        public override List<DomainEvent> DomainEvents
        {
            get => base.DomainEvents;
            set => base.DomainEvents = value;
        }

        public IdTestingDocument PopulateFromEntity(IdTesting entity)
        {
            Identifier = entity.Identifier;
            Id = entity.Id;

            return this;
        }
    }
}