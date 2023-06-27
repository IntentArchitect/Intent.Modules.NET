using System;
using CosmosDB.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : FolderContainer, ICosmosDBDocument<FolderContainerDocument, FolderContainer>
    {
        private string? _type;
        [JsonProperty("id")]
        public new string Id
        {
            get => base.Id ??= Guid.NewGuid().ToString();
            set => base.Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => FolderPartitionKey;

        public static FolderContainerDocument FromEntity(FolderContainer entity)
        {
            if (entity is FolderContainerDocument document)
            {
                return document;
            }

            return new FolderContainerDocument
            {
                Id = entity.Id,
                FolderPartitionKey = entity.FolderPartitionKey
            };
        }
    }
}