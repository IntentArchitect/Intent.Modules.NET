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
    internal class IdTestingDocument : IIdTestingDocument, ICosmosDBDocument<IdTesting, IdTestingDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        private string? etag;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Identifier;
            set => Identifier = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => etag;
        public string Identifier { get; set; } = default!;
        [JsonProperty("@id")]
        public string Id { get; set; } = default!;

        public IdTesting ToEntity(IdTesting? entity = default)
        {
            entity ??= new IdTesting();

            entity.Identifier = Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null");
            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public IdTestingDocument PopulateFromEntity(IdTesting entity, string? etag = null)
        {
            Identifier = entity.Identifier;
            Id = entity.Id;

            this.etag = etag;

            return this;
        }

        public static IdTestingDocument? FromEntity(IdTesting? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new IdTestingDocument().PopulateFromEntity(entity, etag);
        }
    }
}