using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Accounts;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.Accounts;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccTableOverrideRepository : RepositoryBase<AccTableOverride, AccTableOverride, ApplicationDbContext>, IAccTableOverrideRepository
    {
        public AccTableOverrideRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<AccTableOverride?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AccTableOverride>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}