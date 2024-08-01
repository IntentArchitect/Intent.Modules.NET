using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_Poly_ConcreteARepository : RepositoryBase<TPH_Poly_ConcreteA, TPH_Poly_ConcreteA, ApplicationDbContext>, ITPH_Poly_ConcreteARepository
    {
        public TPH_Poly_ConcreteARepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPH_Poly_ConcreteA?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TPH_Poly_ConcreteA>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}