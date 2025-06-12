using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Indexes
{
    public class MultikeyIndexEntitySingleParentMongoRepository : MongoRepositoryBase<MultikeyIndexEntitySingleParent>, IMultikeyIndexEntitySingleParentRepository
    {
        public MultikeyIndexEntitySingleParentMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<MultikeyIndexEntitySingleParent?> FindByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<MultikeyIndexEntitySingleParent>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}