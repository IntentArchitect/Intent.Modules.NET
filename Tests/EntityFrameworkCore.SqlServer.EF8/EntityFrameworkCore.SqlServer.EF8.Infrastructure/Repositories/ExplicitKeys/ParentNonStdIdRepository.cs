using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ParentNonStdIdRepository : RepositoryBase<ParentNonStdId, ParentNonStdId, ApplicationDbContext>, IParentNonStdIdRepository
    {
        public ParentNonStdIdRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ParentNonStdId?> FindByIdAsync(Guid myId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == myId, cancellationToken);
        }

        public async Task<List<ParentNonStdId>> FindByIdsAsync(Guid[] myIds, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => myIds.Contains(x.MyId), cancellationToken);
        }
    }
}