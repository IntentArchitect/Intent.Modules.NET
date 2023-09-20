using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : FolderContainer, ICosmosDBDocument<FolderContainerDocument, FolderContainer>
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
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string IItem.PartitionKey => FolderPartitionKey;
        [JsonIgnore]
        public override List<DomainEvent> DomainEvents
        {
            get => base.DomainEvents;
            set => base.DomainEvents = value;
        }

        public FolderContainerDocument PopulateFromEntity(FolderContainer entity)
        {
            Id = entity.Id;
            FolderPartitionKey = entity.FolderPartitionKey;

            return this;
        }
    }
}