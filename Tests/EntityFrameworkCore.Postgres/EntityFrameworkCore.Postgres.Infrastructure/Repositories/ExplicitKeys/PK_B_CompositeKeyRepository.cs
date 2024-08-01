using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PK_B_CompositeKeyRepository : RepositoryBase<PK_B_CompositeKey, PK_B_CompositeKey, ApplicationDbContext>, IPK_B_CompositeKeyRepository
    {
        public PK_B_CompositeKeyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PK_B_CompositeKey?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }
    }
}