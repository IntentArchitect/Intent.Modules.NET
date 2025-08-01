using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Accounts.NotSchema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.Accounts.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccTableFolderRepository : IEFRepository<AccTableFolder, AccTableFolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccTableFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccTableFolder?> FindByIdAsync(Guid id, Func<IQueryable<AccTableFolder>, IQueryable<AccTableFolder>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccTableFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}