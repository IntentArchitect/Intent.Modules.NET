using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Repositories
{
    internal class CustomerMongoRepository : MongoRepositoryBase<Customer, string>, ICustomerRepository
    {
        public CustomerMongoRepository(IMongoCollection<Customer> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork, x => x.Id)
        {
        }

        public Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => FindAsync(x => x.Id == id, cancellationToken);

        public Task<List<Customer>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default) => FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
    }
}