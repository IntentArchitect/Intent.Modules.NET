using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDecimalKeyDefaultRepository : IEFRepository<DecimalKeyDefault, DecimalKeyDefault>
    {
        [IntentManaged(Mode.Fully)]
        Task<DecimalKeyDefault?> FindByIdAsync(decimal id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DecimalKeyDefault?> FindByIdAsync(decimal id, Func<IQueryable<DecimalKeyDefault>, IQueryable<DecimalKeyDefault>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DecimalKeyDefault>> FindByIdsAsync(decimal[] ids, CancellationToken cancellationToken = default);
    }
}