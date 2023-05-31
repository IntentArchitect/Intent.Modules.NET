using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Indexes;
using MongoDb.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Indexes
{
    public class CompoundIndexEntitySingleParentMongoRepository : MongoRepositoryBase<CompoundIndexEntitySingleParent, CompoundIndexEntitySingleParent>, ICompoundIndexEntitySingleParentRepository
    {
        public CompoundIndexEntitySingleParentMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<CompoundIndexEntitySingleParent> FindByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<CompoundIndexEntitySingleParent>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}