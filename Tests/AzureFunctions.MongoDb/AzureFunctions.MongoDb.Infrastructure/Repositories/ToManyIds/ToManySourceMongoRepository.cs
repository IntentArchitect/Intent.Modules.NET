using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.ToManyIds;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.ToManyIds
{
    public class ToManySourceMongoRepository : MongoRepositoryBase<ToManySource>, IToManySourceRepository
    {
        public ToManySourceMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<ToManySource?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ToManySource>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}