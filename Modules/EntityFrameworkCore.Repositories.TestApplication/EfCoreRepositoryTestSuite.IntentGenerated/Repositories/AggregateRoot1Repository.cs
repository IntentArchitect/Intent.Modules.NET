using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreRepositoryTestSuite.IntentGenerated.Core;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRoot1Repository : RepositoryBase<IAggregateRoot1, AggregateRoot1, ApplicationDbContext>, IAggregateRoot1Repository
    {
        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public AggregateRoot1Repository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }


        [IntentManaged(Mode.Fully)]
        public async Task<IAggregateRoot1> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }


        [IntentManaged(Mode.Fully)]
        public async Task<List<IAggregateRoot1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}