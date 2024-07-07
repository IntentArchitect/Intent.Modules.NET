using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Collections.FolderCollection;
using MongoDb.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Collections.FolderCollection
{
    public class FolderCollectionEntityAMongoRepository : MongoRepositoryBase<FolderCollectionEntityA>, IFolderCollectionEntityARepository
    {
        public FolderCollectionEntityAMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<FolderCollectionEntityA?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<FolderCollectionEntityA>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}