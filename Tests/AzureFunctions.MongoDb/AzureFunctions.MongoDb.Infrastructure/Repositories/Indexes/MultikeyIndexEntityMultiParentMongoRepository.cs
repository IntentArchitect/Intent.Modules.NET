using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Indexes
{
    internal class MultikeyIndexEntityMultiParentMongoRepository : MongoRepositoryBase<MultikeyIndexEntityMultiParent, string>, IMultikeyIndexEntityMultiParentRepository
    {
        public MultikeyIndexEntityMultiParentMongoRepository(IMongoCollection<MultikeyIndexEntityMultiParent> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork, x => x.Id)
        {
        }

        public Task<MultikeyIndexEntityMultiParent?> FindByIdAsync(
            string id,
            CancellationToken cancellationToken = default) => FindAsync(x => x.Id == id, cancellationToken);

        public Task<List<MultikeyIndexEntityMultiParent>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default) => FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
    }
}