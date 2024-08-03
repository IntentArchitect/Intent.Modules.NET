using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPT_ConcreteBaseClassRepository : RepositoryBase<TPT_ConcreteBaseClass, TPT_ConcreteBaseClass, ApplicationDbContext>, ITPT_ConcreteBaseClassRepository
    {
        public TPT_ConcreteBaseClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPT_ConcreteBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TPT_ConcreteBaseClass>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}