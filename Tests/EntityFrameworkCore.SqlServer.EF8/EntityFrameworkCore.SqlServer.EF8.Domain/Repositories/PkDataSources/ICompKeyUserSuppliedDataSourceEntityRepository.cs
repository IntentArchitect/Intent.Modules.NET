using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICompKeyUserSuppliedDataSourceEntityRepository : IEFRepository<CompKeyUserSuppliedDataSourceEntity, CompKeyUserSuppliedDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, Func<IQueryable<CompKeyUserSuppliedDataSourceEntity>, IQueryable<CompKeyUserSuppliedDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
    }
}