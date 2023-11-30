using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class ProductDocument : IProductDocument, ICosmosDBDocument<Product, ProductDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> CategoriesIds { get; set; } = default!;
        IReadOnlyList<string> IProductDocument.CategoriesIds => CategoriesIds;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(CategoriesIds), CategoriesIds ?? throw new Exception($"{nameof(entity.CategoriesIds)} is null"));

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CategoriesIds = entity.CategoriesIds.ToList();

            return this;
        }

        public static ProductDocument? FromEntity(Product? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity);
        }
    }
}