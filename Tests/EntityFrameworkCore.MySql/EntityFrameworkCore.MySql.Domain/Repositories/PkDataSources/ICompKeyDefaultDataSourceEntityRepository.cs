using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICompKeyDefaultDataSourceEntityRepository : IEFRepository<CompKeyDefaultDataSourceEntity, CompKeyDefaultDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<CompKeyDefaultDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CompKeyDefaultDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, Func<IQueryable<CompKeyDefaultDataSourceEntity>, IQueryable<CompKeyDefaultDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
    }
}