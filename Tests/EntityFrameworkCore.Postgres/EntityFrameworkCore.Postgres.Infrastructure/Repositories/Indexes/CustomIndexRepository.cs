using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.Indexes;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomIndexRepository : RepositoryBase<CustomIndex, CustomIndex, ApplicationDbContext>, ICustomIndexRepository
    {
        public CustomIndexRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<CustomIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<CustomIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}