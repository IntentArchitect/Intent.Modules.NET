using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class PackageContainerDocument : IPackageContainerDocument, ICosmosDBDocument<PackageContainer, PackageContainerDocument>
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

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);
            ReflectionHelper.ForceSetProperty(entity, nameof(PackagePartitionKey), PackagePartitionKey);

            return entity;
        }

        public PackageContainerDocument PopulateFromEntity(PackageContainer entity)
        {
            Id = entity.Id;
            PackagePartitionKey = entity.PackagePartitionKey;

            return this;
        }

        public static PackageContainerDocument? FromEntity(PackageContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PackageContainerDocument().PopulateFromEntity(entity);
        }
    }
}