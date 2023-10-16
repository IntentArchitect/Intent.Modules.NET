using System;
using CosmosDB.PrivateSetters.Domain.Entities.Folder;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : IFolderContainerDocument, ICosmosDBDocument<FolderContainer, FolderContainerDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => FolderPartitionKey;
            set => FolderPartitionKey = value!;
        }
        public string Id { get; set; } = default!;
        public string FolderPartitionKey { get; set; } = default!;

        public FolderContainer ToEntity(FolderContainer? entity = default)
        {
            entity ??= new FolderContainer();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(FolderPartitionKey), FolderPartitionKey ?? throw new Exception($"{nameof(entity.FolderPartitionKey)} is null"));

            return entity;
        }

        public FolderContainerDocument PopulateFromEntity(FolderContainer entity)
        {
            Id = entity.Id;
            FolderPartitionKey = entity.FolderPartitionKey;

            return this;
        }

        public static FolderContainerDocument? FromEntity(FolderContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderContainerDocument().PopulateFromEntity(entity);
        }
    }
}