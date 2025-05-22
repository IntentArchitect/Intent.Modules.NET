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
    public interface IDefaultDataSourceEntityRepository : IEFRepository<DefaultDataSourceEntity, DefaultDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<DefaultDataSourceEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultDataSourceEntity?> FindByIdAsync(long id, Func<IQueryable<DefaultDataSourceEntity>, IQueryable<DefaultDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DefaultDataSourceEntity>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}