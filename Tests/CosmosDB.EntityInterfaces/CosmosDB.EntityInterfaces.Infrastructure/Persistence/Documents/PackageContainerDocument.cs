using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class PackageContainerDocument : IPackageContainerDocument, ICosmosDBDocument<IPackageContainer, PackageContainer, PackageContainerDocument>
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
            get => PackagePartitionKey;
            set => PackagePartitionKey = value!;
        }
        public string Id { get; set; } = default!;
        public string PackagePartitionKey { get; set; } = default!;

        public PackageContainer ToEntity(PackageContainer? entity = default)
        {
            entity ??= new PackageContainer();

            entity.Id = Id;
            entity.PackagePartitionKey = PackagePartitionKey;

            return entity;
        }

        public PackageContainerDocument PopulateFromEntity(IPackageContainer entity)
        {
            Id = entity.Id;
            PackagePartitionKey = entity.PackagePartitionKey;

            return this;
        }

        public static PackageContainerDocument? FromEntity(IPackageContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PackageContainerDocument().PopulateFromEntity(entity);
        }
    }
}