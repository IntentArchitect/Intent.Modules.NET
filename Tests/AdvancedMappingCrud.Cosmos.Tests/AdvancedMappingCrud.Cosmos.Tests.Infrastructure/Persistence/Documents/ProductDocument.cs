using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
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

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity)
        {
            Id = entity.Id;
            Name = entity.Name;

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