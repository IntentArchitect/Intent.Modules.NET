using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class PackageContainerDocument : IPackageContainerDocument, ICosmosDBDocument<PackageContainer, PackageContainerDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
        public string Id { get; set; } = default!;
        public string PackagePartitionKey { get; set; } = default!;

        public PackageContainer ToEntity(PackageContainer? entity = default)
        {
            entity ??= new PackageContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PackagePartitionKey = PackagePartitionKey ?? throw new Exception($"{nameof(entity.PackagePartitionKey)} is null");

            return entity;
        }
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

        public PackageContainerDocument PopulateFromEntity(PackageContainer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            PackagePartitionKey = entity.PackagePartitionKey;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static PackageContainerDocument? FromEntity(PackageContainer? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new PackageContainerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}