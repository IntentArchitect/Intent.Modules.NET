using System;
using System.Linq;
using System.Linq.Expressions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents
{
    internal class CustomerDocument : ICustomerDocument, IMongoDbDocument<Customer, CustomerDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Surname = Surname ?? throw new Exception($"{nameof(entity.Surname)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Surname = entity.Surname;
            Email = entity.Email;

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