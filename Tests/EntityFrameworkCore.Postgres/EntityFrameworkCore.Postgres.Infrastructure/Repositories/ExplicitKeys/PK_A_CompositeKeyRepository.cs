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
    public class PK_A_CompositeKeyRepository : RepositoryBase<PK_A_CompositeKey, PK_A_CompositeKey, ApplicationDbContext>, IPK_A_CompositeKeyRepository
    {
        public PK_A_CompositeKeyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PK_A_CompositeKey?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }
    }
}