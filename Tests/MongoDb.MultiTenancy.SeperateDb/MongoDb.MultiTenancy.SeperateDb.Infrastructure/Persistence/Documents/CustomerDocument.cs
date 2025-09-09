using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.MultiTenancy.SeperateDb.Domain.Entities;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(Customer), Required = true)]
    internal class CustomerDocument : ICustomerDocument, IMongoDbDocument<Customer, CustomerDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CustomerDocument? FromEntity(Customer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CustomerDocument> GetIdFilter(string id)
        {
            return Builders<CustomerDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CustomerDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CustomerDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CustomerDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<CustomerDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CustomerDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}