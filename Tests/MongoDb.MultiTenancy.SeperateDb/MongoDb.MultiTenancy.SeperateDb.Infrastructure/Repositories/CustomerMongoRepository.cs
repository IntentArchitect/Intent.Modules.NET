using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.MultiTenancy.SeperateDb.Domain.Entities;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories.Documents;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence;
using MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Repositories
{
    internal class CustomerMongoRepository : MongoRepositoryBase<Customer, CustomerDocument, ICustomerDocument, string>, ICustomerRepository
    {
        public CustomerMongoRepository(IMongoCollection<CustomerDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}