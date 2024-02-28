using System;
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
        [JsonProperty("_etag")]
        protected string? _etag;
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
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string PackagePartitionKey { get; set; } = default!;

        public PackageContainer ToEntity(PackageContainer? entity = default)
        {
            entity ??= new PackageContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PackagePartitionKey = PackagePartitionKey ?? throw new Exception($"{nameof(entity.PackagePartitionKey)} is null");

            return entity;
        }

        public PackageContainerDocument PopulateFromEntity(IPackageContainer entity, string? etag = null)
        {
            Id = entity.Id;
            PackagePartitionKey = entity.PackagePartitionKey;

            _etag = etag;

            return this;
        }

        public static PackageContainerDocument? FromEntity(IPackageContainer? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new PackageContainerDocument().PopulateFromEntity(entity, etag);
        }
    }
}