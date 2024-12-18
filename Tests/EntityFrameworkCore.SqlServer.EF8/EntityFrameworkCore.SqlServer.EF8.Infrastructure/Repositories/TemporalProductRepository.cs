using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TemporalProductRepository : RepositoryBase<TemporalProduct, TemporalProduct, ApplicationDbContext>, ITemporalProductRepository
    {
        public TemporalProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TemporalProduct?> FindByIdAsync(Guid id1, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id1, cancellationToken);
        }

        public async Task<List<TemporalProduct>> FindByIdsAsync(Guid[] id1s, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => id1s.Contains(x.Id1), cancellationToken);
        }
    }
}