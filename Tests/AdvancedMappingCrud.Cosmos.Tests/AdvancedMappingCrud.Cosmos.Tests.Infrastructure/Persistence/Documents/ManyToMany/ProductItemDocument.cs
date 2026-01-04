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
    internal class ProductItemDocument : IProductItemDocument, ICosmosDBDocument<ProductItem, ProductItemDocument>
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
        public List<Guid> CategoriesIds { get; set; } = default!;
        IReadOnlyList<Guid> IProductItemDocument.CategoriesIds => CategoriesIds;
        public List<Guid> TagsIds { get; set; } = default!;
        IReadOnlyList<Guid> IProductItemDocument.TagsIds => TagsIds;

        public ProductItem ToEntity(ProductItem? entity = default)
        {
            entity ??= new ProductItem();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.CategoriesIds = CategoriesIds ?? throw new Exception($"{nameof(entity.CategoriesIds)} is null");
            entity.TagsIds = TagsIds ?? throw new Exception($"{nameof(entity.TagsIds)} is null");

            return entity;
        }

        public ProductItemDocument PopulateFromEntity(ProductItem entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;
            CategoriesIds = entity.CategoriesIds.ToList();
            TagsIds = entity.TagsIds.ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ProductItemDocument? FromEntity(ProductItem? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductItemDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}