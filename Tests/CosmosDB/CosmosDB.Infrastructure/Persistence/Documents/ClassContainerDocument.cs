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
    internal class ClassContainerDocument : IClassContainerDocument, ICosmosDBDocument<ClassContainer, ClassContainerDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        private string? _etag;
        public string Id { get; set; } = default!;
        public string ClassPartitionKey { get; set; } = default!;

        public ClassContainer ToEntity(ClassContainer? entity = default)
        {
            entity ??= new ClassContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ClassPartitionKey = ClassPartitionKey ?? throw new Exception($"{nameof(entity.ClassPartitionKey)} is null");

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
            get => ClassPartitionKey;
            set => ClassPartitionKey = value!;
        }
        string? IItemWithEtag.Etag => _etag;

        public ClassContainerDocument PopulateFromEntity(ClassContainer entity, string? etag = null)
        {
            Id = entity.Id;
            ClassPartitionKey = entity.ClassPartitionKey;

            _etag = etag;

            return this;
        }

        public static ClassContainerDocument? FromEntity(ClassContainer? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClassContainerDocument().PopulateFromEntity(entity, etag);
        }
    }
}