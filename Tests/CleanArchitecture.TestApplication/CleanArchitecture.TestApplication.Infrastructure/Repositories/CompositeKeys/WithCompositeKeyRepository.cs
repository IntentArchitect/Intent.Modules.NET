using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CompositeKeys;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CompositeKeys;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Repositories.CompositeKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class WithCompositeKeyRepository : RepositoryBase<WithCompositeKey, WithCompositeKey, ApplicationDbContext>, IWithCompositeKeyRepository
    {
        public WithCompositeKeyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<WithCompositeKey?> FindByIdAsync(
            (Guid Key1Id, Guid Key2Id) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Key1Id == id.Key1Id && x.Key2Id == id.Key2Id, cancellationToken);
        }
    }
}