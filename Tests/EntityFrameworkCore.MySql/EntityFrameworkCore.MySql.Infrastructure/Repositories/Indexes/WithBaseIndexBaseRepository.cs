using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Indexes;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.Indexes;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class WithBaseIndexBaseRepository : RepositoryBase<WithBaseIndexBase, WithBaseIndexBase, ApplicationDbContext>, IWithBaseIndexBaseRepository
    {
        public WithBaseIndexBaseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<WithBaseIndexBase?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<WithBaseIndexBase>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}