using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Repositories.UniqueIndexConstraint
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateWithUniqueConstraintIndexElementRepository : RepositoryBase<AggregateWithUniqueConstraintIndexElement, AggregateWithUniqueConstraintIndexElement, ApplicationDbContext>, IAggregateWithUniqueConstraintIndexElementRepository
    {
        public AggregateWithUniqueConstraintIndexElementRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<AggregateWithUniqueConstraintIndexElement?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AggregateWithUniqueConstraintIndexElement>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}