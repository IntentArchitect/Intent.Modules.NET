using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities.Folder;
using CosmosDB.Domain.Repositories.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : IFolderContainerDocument, ICosmosDBDocument<FolderContainer, FolderContainerDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
        public string Id { get; set; } = default!;
        public string FolderPartitionKey { get; set; } = default!;

        public FolderContainer ToEntity(FolderContainer? entity = default)
        {
            entity ??= new FolderContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.FolderPartitionKey = FolderPartitionKey ?? throw new Exception($"{nameof(entity.FolderPartitionKey)} is null");

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
            get => FolderPartitionKey;
            set => FolderPartitionKey = value!;
        }
        string? IItemWithEtag.Etag => _etag;

        public FolderContainerDocument PopulateFromEntity(FolderContainer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            FolderPartitionKey = entity.FolderPartitionKey;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static FolderContainerDocument? FromEntity(FolderContainer? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderContainerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}