using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Persistence.Documents
{
    internal class CustomerMongoDocument : ICustomerMongoDocument, IMongoDbDocument<CustomerMongo, CustomerMongoDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }

        public CustomerMongo ToEntity(CustomerMongo? entity = default)
        {
            entity ??= new CustomerMongo();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CustomerMongoDocument PopulateFromEntity(CustomerMongo entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CustomerMongoDocument? FromEntity(CustomerMongo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerMongoDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CustomerMongoDocument> GetIdFilter(string id)
        {
            return Builders<CustomerMongoDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CustomerMongoDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CustomerMongoDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CustomerMongoDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<CustomerMongoDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CustomerMongoDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}