using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Accounts.NotSchema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.Accounts.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccViewFolderRepository : IEFRepository<AccViewFolder, AccViewFolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccViewFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccViewFolder?> FindByIdAsync(Guid id, Func<IQueryable<AccViewFolder>, IQueryable<AccViewFolder>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccViewFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}