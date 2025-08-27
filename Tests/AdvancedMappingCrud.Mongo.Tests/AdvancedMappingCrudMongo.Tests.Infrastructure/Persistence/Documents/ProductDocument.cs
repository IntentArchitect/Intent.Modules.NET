using System;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents
{
    internal class ProductDocument : IProductDocument, IMongoDbDocument<Product, ProductDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;

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

        public static FilterDefinition<ProductDocument> GetIdFilter(string id)
        {
            return Builders<ProductDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<ProductDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<ProductDocument> GetIdsFilter(string[] ids)
        {
            return Builders<ProductDocument>.Filter.In(d => d.Id, ids);
        }
    }
}