using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPC_DerivedClassForConcreteAssociatedRepository : RepositoryBase<TPC_DerivedClassForConcreteAssociated, TPC_DerivedClassForConcreteAssociated, ApplicationDbContext>, ITPC_DerivedClassForConcreteAssociatedRepository
    {
        public TPC_DerivedClassForConcreteAssociatedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPC_DerivedClassForConcreteAssociated?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TPC_DerivedClassForConcreteAssociated>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}