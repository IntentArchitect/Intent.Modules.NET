using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPC_DerivedClassForConcreteAssociatedRepository : RepositoryBase<TPC_DerivedClassForConcreteAssociated, TPC_DerivedClassForConcreteAssociated, ApplicationDbContext>, ITPC_DerivedClassForConcreteAssociatedRepository
    {
        public TPC_DerivedClassForConcreteAssociatedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPC_DerivedClassForConcreteAssociated> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Id not found.");
        }

        public async Task<List<TPC_DerivedClassForConcreteAssociated>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}