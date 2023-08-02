using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.OperationAndConstructorMapping;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.OperationAndConstructorMapping;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Repositories.OperationAndConstructorMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OpAndCtorMapping3Repository : RepositoryBase<OpAndCtorMapping3, OpAndCtorMapping3, ApplicationDbContext>, IOpAndCtorMapping3Repository
    {
        public OpAndCtorMapping3Repository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<OpAndCtorMapping3?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<OpAndCtorMapping3>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}