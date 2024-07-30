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
    internal class ClassContainerDocument : IClassContainerDocument, ICosmosDBDocument<IClassContainer, ClassContainer, ClassContainerDocument>
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
            get => ClassPartitionKey;
            set => ClassPartitionKey = value!;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string ClassPartitionKey { get; set; } = default!;

        public ClassContainer ToEntity(ClassContainer? entity = default)
        {
            entity ??= new ClassContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ClassPartitionKey = ClassPartitionKey ?? throw new Exception($"{nameof(entity.ClassPartitionKey)} is null");

            return entity;
        }

        public ClassContainerDocument PopulateFromEntity(IClassContainer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            ClassPartitionKey = entity.ClassPartitionKey;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ClassContainerDocument? FromEntity(IClassContainer? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClassContainerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}