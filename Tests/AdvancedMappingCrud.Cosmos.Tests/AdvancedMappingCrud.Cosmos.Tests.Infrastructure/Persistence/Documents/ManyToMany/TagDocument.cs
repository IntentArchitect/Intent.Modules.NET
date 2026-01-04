using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents.ManyToMany
{
    internal class TagDocument : ITagDocument, ICosmosDBDocument<Tag, TagDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Guid> ProductItemsIds { get; set; } = default!;
        IReadOnlyList<Guid> ITagDocument.ProductItemsIds => ProductItemsIds;

        public Tag ToEntity(Tag? entity = default)
        {
            entity ??= new Tag();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.ProductItemsIds = ProductItemsIds ?? throw new Exception($"{nameof(entity.ProductItemsIds)} is null");

            return entity;
        }

        public TagDocument PopulateFromEntity(Tag entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;
            ProductItemsIds = entity.ProductItemsIds.ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static TagDocument? FromEntity(Tag? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new TagDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}