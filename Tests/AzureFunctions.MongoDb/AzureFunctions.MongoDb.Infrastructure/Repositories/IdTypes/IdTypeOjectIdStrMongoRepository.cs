using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.IdTypes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.IdTypes
{
    public class IdTypeOjectIdStrMongoRepository : MongoRepositoryBase<IdTypeOjectIdStr>, IIdTypeOjectIdStrRepository
    {
        public IdTypeOjectIdStrMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<IdTypeOjectIdStr?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<IdTypeOjectIdStr>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}