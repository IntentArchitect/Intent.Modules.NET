using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_FkBaseClassRepository : RepositoryBase<TPH_FkBaseClass, TPH_FkBaseClass, ApplicationDbContext>, ITPH_FkBaseClassRepository
    {
        public TPH_FkBaseClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPH_FkBaseClass?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }

        public async Task<TPH_FkBaseClass?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            Func<IQueryable<TPH_FkBaseClass>, IQueryable<TPH_FkBaseClass>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, queryOptions, cancellationToken);
        }
    }
}