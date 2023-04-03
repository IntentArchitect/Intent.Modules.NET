using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.IdTypes
{
    public class IdTypeLongRepository : MongoRepositoryBase<IdTypeLong, IdTypeLong>, IIdTypeLongRepository
    {
        public IdTypeLongRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<IdTypeLong> FindByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<IdTypeLong>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}