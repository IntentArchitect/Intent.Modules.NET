using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ProductDocument : IProductDocument, ICosmosDBDocument<Product, ProductDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        private string? etag;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> CategoriesIds { get; set; } = default!;
        IReadOnlyList<string> IProductDocument.CategoriesIds => CategoriesIds;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.CategoriesIds = CategoriesIds ?? throw new Exception($"{nameof(entity.CategoriesIds)} is null");

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity, string? etag = null)
        {
            Id = entity.Id;
            Name = entity.Name;
            CategoriesIds = entity.CategoriesIds.ToList();

            this.etag = etag;

            return this;
        }

        public static ProductDocument? FromEntity(Product? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity, etag);
        }
    }
}