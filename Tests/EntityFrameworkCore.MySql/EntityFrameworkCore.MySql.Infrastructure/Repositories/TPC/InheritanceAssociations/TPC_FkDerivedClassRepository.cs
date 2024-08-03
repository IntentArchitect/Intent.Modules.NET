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
    public class TPC_FkDerivedClassRepository : RepositoryBase<TPC_FkDerivedClass, TPC_FkDerivedClass, ApplicationDbContext>, ITPC_FkDerivedClassRepository
    {
        public TPC_FkDerivedClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPC_FkDerivedClass?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }
    }
}