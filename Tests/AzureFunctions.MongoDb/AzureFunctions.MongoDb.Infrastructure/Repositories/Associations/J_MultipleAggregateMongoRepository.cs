using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Associations;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Associations
{
    internal class J_MultipleAggregateMongoRepository : MongoRepositoryBase<J_MultipleAggregate, string>, IJ_MultipleAggregateRepository
    {
        public J_MultipleAggregateMongoRepository(IMongoCollection<J_MultipleAggregate> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork, x => x.Id)
        {
        }

        public Task<J_MultipleAggregate?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => FindAsync(x => x.Id == id, cancellationToken);

        public Task<List<J_MultipleAggregate>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default) => FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
    }
}