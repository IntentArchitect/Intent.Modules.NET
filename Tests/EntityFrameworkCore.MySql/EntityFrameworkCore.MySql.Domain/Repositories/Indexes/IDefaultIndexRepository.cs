using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDefaultIndexRepository : IEFRepository<DefaultIndex, DefaultIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<DefaultIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultIndex?> FindByIdAsync(Guid id, Func<IQueryable<DefaultIndex>, IQueryable<DefaultIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DefaultIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}