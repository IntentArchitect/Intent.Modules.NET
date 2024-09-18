using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Entities;
using CosmosDB.EnumStrings.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Persistence.Documents
{
    internal class RootEntityDocument : IRootEntityDocument, ICosmosDBDocument<RootEntity, RootEntityDocument>
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
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample EnumExample { get; set; }
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample? NullableEnumExample { get; set; }
        public EmbeddedObjectDocument Embedded { get; set; } = default!;
        IEmbeddedObjectDocument IRootEntityDocument.Embedded => Embedded;
        public List<NestedEntityDocument> NestedEntities { get; set; } = default!;
        IReadOnlyList<INestedEntityDocument> IRootEntityDocument.NestedEntities => NestedEntities;

        public RootEntity ToEntity(RootEntity? entity = default)
        {
            entity ??= new RootEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.EnumExample = EnumExample;
            entity.NullableEnumExample = NullableEnumExample;
            entity.Embedded = Embedded.ToEntity() ?? throw new Exception($"{nameof(entity.Embedded)} is null");
            entity.NestedEntities = NestedEntities.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public RootEntityDocument PopulateFromEntity(RootEntity entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            EnumExample = entity.EnumExample;
            NullableEnumExample = entity.NullableEnumExample;
            Embedded = EmbeddedObjectDocument.FromEntity(entity.Embedded)!;
            NestedEntities = entity.NestedEntities.Select(x => NestedEntityDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static RootEntityDocument? FromEntity(RootEntity? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new RootEntityDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}