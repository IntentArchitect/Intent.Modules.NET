using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitKeyAggrRootRepository : RepositoryBase<ImplicitKeyAggrRoot, ImplicitKeyAggrRoot, ApplicationDbContext>, IImplicitKeyAggrRootRepository
    {
        public ImplicitKeyAggrRootRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ImplicitKeyAggrRoot> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ImplicitKeyAggrRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}