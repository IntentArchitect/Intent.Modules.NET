using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.Repository", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Repositories
{
    public class PrivilegeMongoRepository : MongoRepositoryBase<Privilege>, IPrivilegeRepository
    {
        public PrivilegeMongoRepository(ApplicationMongoDbContext context) : base(context)
        {
        }

        public async Task<Privilege?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Privilege>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}