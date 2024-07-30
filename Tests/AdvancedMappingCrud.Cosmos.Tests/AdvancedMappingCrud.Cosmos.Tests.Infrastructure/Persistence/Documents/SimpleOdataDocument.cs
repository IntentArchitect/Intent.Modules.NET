using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class SimpleOdataDocument : ISimpleOdataDocument, ICosmosDBDocument<SimpleOdata, SimpleOdataDocument>
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
        public string Surname { get; set; } = default!;
        public List<SimpleChildDocument> SimpleChildren { get; set; } = default!;
        IReadOnlyList<ISimpleChildDocument> ISimpleOdataDocument.SimpleChildren => SimpleChildren;

        public SimpleOdata ToEntity(SimpleOdata? entity = default)
        {
            entity ??= new SimpleOdata();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Surname = Surname ?? throw new Exception($"{nameof(entity.Surname)} is null");
            entity.SimpleChildren = SimpleChildren.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public SimpleOdataDocument PopulateFromEntity(SimpleOdata entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Surname = entity.Surname;
            SimpleChildren = entity.SimpleChildren.Select(x => SimpleChildDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static SimpleOdataDocument? FromEntity(SimpleOdata? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new SimpleOdataDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}