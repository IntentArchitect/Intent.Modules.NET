using CosmosDB.EntityInterfaces.Domain.Entities.Folder;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : IFolderContainerDocument, ICosmosDBDocument<IFolderContainer, FolderContainer, FolderContainerDocument>
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

            entity.Id = Id;
            entity.FolderPartitionKey = FolderPartitionKey;

            return entity;
        }

        public FolderContainerDocument PopulateFromEntity(IFolderContainer entity)
        {
            Id = entity.Id;
            FolderPartitionKey = entity.FolderPartitionKey;

            return this;
        }

        public static FolderContainerDocument? FromEntity(IFolderContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderContainerDocument().PopulateFromEntity(entity);
        }
    }
}